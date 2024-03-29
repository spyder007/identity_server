﻿using System.Reflection;

using Duende.IdentityServer;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Spydersoft.Identity.Data;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.Identity;
using Spydersoft.Identity.Options;
using Spydersoft.Identity.Services;

namespace Spydersoft.Identity
{
    /// <summary>
    /// Class Startup.
    /// </summary>
    public class Startup(IConfiguration configuration)
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var connString = Configuration.GetConnectionString("IdentityConnection");
            var cacheConnection = Configuration.GetConnectionString("RedisCache");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            _ = services.Configure<SendgridOptions>(Configuration.GetSection(SendgridOptions.Name));
            _ = services.Configure<ConsentOptions>(Configuration.GetSection(ConsentOptions.SettingsKey));

            var zipkinSettings = new ZipkinOptions();
            Configuration.GetSection(ZipkinOptions.SettingsKey).Bind(zipkinSettings);

            _ = services.AddOpenTelemetry()
                .WithTracing(builder => _ = builder
                    .AddZipkinExporter(config => config.Endpoint = new System.Uri(zipkinSettings.Host))
                    .AddSource(IdentityServerConstants.Tracing.Basic)
                    .AddSource(IdentityServerConstants.Tracing.Cache)
                    .AddSource(IdentityServerConstants.Tracing.Services)
                    .AddSource(IdentityServerConstants.Tracing.Stores)
                    .AddSource(IdentityServerConstants.Tracing.Validation)

                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(zipkinSettings.ServiceName))
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddSqlClientInstrumentation())

                .WithMetrics(builder => _ = builder.AddPrometheusExporter()
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation());

            _ = services.ConfigureNonBreakingSameSiteCookies();
            _ = services.AddHttpContextAccessor();
            // Add framework services.
            _ = services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connString));

            _ = services.AddDbContext<DataProtectionDbContext>(options => options.UseSqlServer(connString));
            _ = services.AddDataProtection()
                .SetApplicationName("identity-server")
                .PersistKeysToDbContext<DataProtectionDbContext>();

            if (!string.IsNullOrEmpty(cacheConnection))
            {
                _ = services.AddStackExchangeRedisCache(options => options.Configuration = cacheConnection);
            }

            _ = services.AddAutoMapper(typeof(Startup));
            _ = services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.User.RequireUniqueEmail = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            _ = services.AddTransient<IEmailSender, EmailSender>();

            _ = services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
                options.EnableEndpointRouting = false;
            });

            // this adds the Configuration Store (clients, resources) and then
            // the Operation Store (codes, tokens, consents)
            _ = services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options => options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connString,
                            sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                });

            var providerSettings = new ProviderOptions();
            Configuration.GetSection(ProviderOptions.SettingsKey).Bind(providerSettings);
            _ = services.AddAuthentication()
                .AddGoogle(option =>
                {
                    option.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    option.ClientId = providerSettings.GoogleClientId;
                    option.ClientSecret = providerSettings.GoogleClientSecret;
                });
            _ = services.AddAuthorization();
            _ = services.AddHealthChecks()
                .AddSqlServer(connString, null, null, "sqlserver", null, ["ready"], null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // this will do the initial DB population, but we only need to do it once
            // this is just in here as a easy, yet hacky, way to get our DB created/populated
            var dbInitialize = new DatabaseInitializer(app);
            dbInitialize.InitializeDatabase();

            _ = env.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseExceptionHandler("/Home/Error");

            _ = app.UseHealthChecks("/healthz", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
            _ = app.UseHealthChecks("/readyz", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
            _ = app.UseHealthChecks("/livez", new HealthCheckOptions { Predicate = _ => false });
            _ = app.UseCookiePolicy();
            _ = app.UseStaticFiles();
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();

            _ = app.UseForwardedHeaders(forwardedHeadersOptions);
            _ = app.UseOpenTelemetryPrometheusScrapingEndpoint();
            _ = app.UseAuthentication();
            _ = app.UseRouting();
            _ = app.UseIdentityServer();
            _ = app.UseAuthorization();
            _ = app.UseEndpoints(endpoints => _ = endpoints.MapControllers());

            _ = app.UseMvc(routes => _ = routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" }));
        }


    }
}
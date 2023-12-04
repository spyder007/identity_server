using System.Reflection;

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
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connString = Configuration.GetConnectionString("IdentityConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            _ = services.Configure<SendgridOptions>(Configuration.GetSection(SendgridOptions.Name));
            _ = services.Configure<ConsentOptions>(Configuration.GetSection(ConsentOptions.SettingsKey));

            _ = services.AddOpenTelemetry()
                .WithTracing(builder => _ = builder
                    .AddZipkinExporter(config => config.Endpoint = new System.Uri(Configuration.GetValue<string>("Zipkin:Host")))
                    .AddSource(IdentityServerConstants.Tracing.Basic)
                    .AddSource(IdentityServerConstants.Tracing.Cache)
                    .AddSource(IdentityServerConstants.Tracing.Services)
                    .AddSource(IdentityServerConstants.Tracing.Stores)
                    .AddSource(IdentityServerConstants.Tracing.Validation)

                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(Configuration.GetValue<string>("Zipkin:ServiceName")))
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

            var cacheConnection = Configuration.GetConnectionString("RedisCache");
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

            _ = services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options => options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connString,
                            sql => sql.MigrationsAssembly(migrationsAssembly)))
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                });

            _ = services.AddAuthentication()
                .AddGoogle(option =>
                {
                    option.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    option.ClientId = Configuration.GetValue<string>("ProviderSettings:GoogleClientId");
                    option.ClientSecret = Configuration.GetValue<string>("ProviderSettings:GoogleClientSecret");
                });
            _ = services.AddAuthorization();
            _ = services.AddHealthChecks()
                .AddSqlServer(connString, null, null, "sqlserver", null, new[] { "ready" }, null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
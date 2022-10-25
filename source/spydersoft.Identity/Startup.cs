﻿using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using spydersoft.Identity.Data;
using spydersoft.Identity.Models;
using spydersoft.Identity.Services;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Duende.IdentityServer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using spydersoft.Identity.Extensions;
using spydersoft.Identity.Models.Identity;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.DataProtection;

namespace spydersoft.Identity
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
            services.AddOpenTelemetryTracing(builder =>
            {
                builder
                    .AddZipkinExporter(config =>
                    {
                        config.Endpoint = new System.Uri(Configuration.GetValue<string>("Zipkin:Host"));
                    })
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
                    .AddSqlClientInstrumentation();
            });

            services.AddOpenTelemetryMetrics(builder =>
            {
                builder.AddPrometheusExporter()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();
            });

            services.ConfigureNonBreakingSameSiteCookies();
            services.AddHttpContextAccessor();
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connString));
            
            services.AddDbContext<DataProtectionDbContext>(options => options.UseSqlServer(connString));
            services.AddDataProtection()
                .SetApplicationName("identity-server")
                .PersistKeysToDbContext<DataProtectionDbContext>();

            var cacheConnection = Configuration.GetConnectionString("RedisCache");
            if (!string.IsNullOrEmpty(cacheConnection))
            {
                services.AddStackExchangeRedisCache(options => { options.Configuration = cacheConnection; });
            }

            services.AddAutoMapper(typeof(Startup));
            //services.AddSingleton(Data.AutoMapper.GetMapperConfiguration());
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
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

            services.AddAuthentication()
                .AddGoogle(option =>
                {
                    option.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    option.ClientId = Configuration.GetValue<string>("ProviderSettings:GoogleClientId");
                    option.ClientSecret = Configuration.GetValue<string>("ProviderSettings:GoogleClientSecret");
                });
            services.AddHealthChecks()
                .AddSqlServer(connString, null, "sqlserver", null, new[] { "ready" }, null, null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // this will do the initial DB population, but we only need to do it once
            // this is just in here as a easy, yet hacky, way to get our DB created/populated
            var dbInitialize = new DatabaseInitializer(app);
            dbInitialize.InitializeDatabase();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseHealthChecks("/healthz", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
            app.UseHealthChecks("/readyz", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
            app.UseHealthChecks("/livez", new HealthCheckOptions { Predicate = _ => false });
            app.UseCookiePolicy();
            app.UseStaticFiles();
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardedHeadersOptions);
            app.UseOpenTelemetryPrometheusScrapingEndpoint();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            // app.UseIdentity(); // not needed, since UseIdentityServer adds the authentication middleware
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }


    }
}
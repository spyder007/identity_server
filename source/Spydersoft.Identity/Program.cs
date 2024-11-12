using System;
using System.Reflection;

using Duende.IdentityServer;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Trace;

using Serilog;

using Spydersoft.Identity.Data;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Models.Identity;
using Spydersoft.Identity.Options;
using Spydersoft.Identity.Services;
using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.StartupExtensions;

IConfigurationRoot baseConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(baseConfig)
    .CreateBootstrapLogger();

try
{
    Log.Information("identityServer starting.");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.AddSpydersoftTelemetry(typeof(Program).Assembly,
        additionalTraceConfiguration: (telemetry) => telemetry
            .AddSource(IdentityServerConstants.Tracing.Basic)
            .AddSource(IdentityServerConstants.Tracing.Cache)
            .AddSource(IdentityServerConstants.Tracing.Services)
            .AddSource(IdentityServerConstants.Tracing.Stores)
            .AddSource(IdentityServerConstants.Tracing.Validation)
            .AddSqlClientInstrumentation(),
        additionalMetricsConfiguration: null,
        additionalLogConfiguration: null)
        .AddSpydersoftSerilog();

    AppHealthCheckOptions healthCheckOptions = builder.AddSpydersoftHealthChecks();

    var connString = builder.Configuration.GetConnectionString("IdentityConnection");
    var cacheConnection = builder.Configuration.GetConnectionString("RedisCache");
    var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

    _ = builder.Services.Configure<SendgridOptions>(builder.Configuration.GetSection(SendgridOptions.Name));
    _ = builder.Services.Configure<ConsentOptions>(builder.Configuration.GetSection(ConsentOptions.SettingsKey));

    _ = builder.Services.ConfigureNonBreakingSameSiteCookies();
    _ = builder.Services.AddHttpContextAccessor();
    // Add framework builder.Services.
    _ = builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connString));

    _ = builder.Services.AddDbContext<DataProtectionDbContext>(options => options.UseSqlServer(connString));
    _ = builder.Services.AddDataProtection()
        .SetApplicationName("identity-server")
        .PersistKeysToDbContext<DataProtectionDbContext>();

    if (!string.IsNullOrEmpty(cacheConnection))
    {
        _ = builder.Services.AddStackExchangeRedisCache(options => options.Configuration = cacheConnection);
    }

    _ = builder.Services.AddAutoMapper(typeof(Program));
    _ = builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.User.RequireUniqueEmail = true)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    // Add application builder.Services.
    _ = builder.Services.AddTransient<IEmailSender, EmailSender>();

    _ = builder.Services.AddMvc(options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
        options.EnableEndpointRouting = false;
    });

    // this adds the Configuration Store (clients, resources) and then
    // the Operation Store (codes, tokens, consents)
    _ = builder.Services.AddIdentityServer()
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
    builder.Configuration.GetSection(ProviderOptions.SettingsKey).Bind(providerSettings);
    _ = builder.Services.AddAuthentication()
        .AddGoogle(option =>
        {
            option.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            option.ClientId = providerSettings.GoogleClientId;
            option.ClientSecret = providerSettings.GoogleClientSecret;
        });
    _ = builder.Services.AddAuthorization();
    _ = builder.Services.AddHealthChecks()
        .AddSqlServer(connString, null, null, "sqlserver", null, ["ready"], null);

    WebApplication app = builder.Build();
    // this will do the initial DB population, but we only need to do it once
    // this is just in here as a easy, yet hacky, way to get our DB created/populated
    var dbInitialize = new DatabaseInitializer(app);
    dbInitialize.InitializeDatabase();

    _ = builder.Environment.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseExceptionHandler("/Home/Error");

    _ = app.UseSpydersoftHealthChecks(healthCheckOptions)
            .UseCookiePolicy()
            .UseStaticFiles();
    var forwardedHeadersOptions = new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    };
    forwardedHeadersOptions.KnownNetworks.Clear();
    forwardedHeadersOptions.KnownProxies.Clear();

    _ = app.UseForwardedHeaders(forwardedHeadersOptions)
        .UseOpenTelemetryPrometheusScrapingEndpoint()
        .UseAuthentication()
        .UseRouting()
        .UseIdentityServer()
        .UseAuthorization()
        .UseEndpoints(endpoints => _ = endpoints.MapControllers());

    _ = app.UseMvc(routes => _ = routes.MapRoute(
            name: "default",
            template: "{controller}/{action}/{id?}",
            defaults: new { controller = "Home", action = "Index" }));

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "identityServer failed to start.");
}
finally
{
    Log.Information("identityServer shut down complete");
    await Log.CloseAndFlushAsync();
}
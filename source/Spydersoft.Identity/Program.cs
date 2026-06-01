using System;
using System.Linq;

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

using Npgsql;

using Serilog;

using Spydersoft.Identity.Attributes;
using Spydersoft.Identity.Data;
using Spydersoft.Identity.Core.Extensions;
using Spydersoft.Identity.Extensions;
using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Options;
using Spydersoft.Identity.Core.Services;
using Spydersoft.Identity.Services;
using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.StartupExtensions;
using Spydersoft.Platform.Hosting.Telemetry;

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
        new ConfigurationFunctions
        {
            TraceConfiguration = telemetry => telemetry
                .AddSource(IdentityServerConstants.Tracing.Basic)
                .AddSource(IdentityServerConstants.Tracing.Cache)
                .AddSource(IdentityServerConstants.Tracing.Services)
                .AddSource(IdentityServerConstants.Tracing.Stores)
                .AddSource(IdentityServerConstants.Tracing.Validation)
                .AddNpgsql(),
            MetricsConfiguration = metrics => metrics
                .AddNpgsqlInstrumentation()
        })
        .AddSpydersoftSerilog(true);

    AppHealthCheckOptions healthCheckOptions = builder.AddSpydersoftHealthChecks();

    var connString = builder.Configuration.GetConnectionString("IdentityConnection");
    var cacheConnection = builder.Configuration.GetConnectionString("RedisCache");
    var migrationsAssembly = typeof(Spydersoft.Identity.Data.ApplicationDbContext).Assembly.GetName().Name;

    _ = builder.Services.Configure<SendgridOptions>(builder.Configuration.GetSection(SendgridOptions.Name));
    _ = builder.Services.Configure<ConsentOptions>(builder.Configuration.GetSection(ConsentOptions.SettingsKey));

    // Configure forwarded headers for proxy scenarios
    _ = builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                                   ForwardedHeaders.XForwardedProto | 
                                   ForwardedHeaders.XForwardedHost;
        // Clear known networks and proxies to accept any proxy
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
        // Trust all proxies - adjust this based on your security requirements
        options.ForwardLimit = null;
    });

    _ = builder.Services.ConfigureNonBreakingSameSiteCookies();
    _ = builder.Services.AddHttpContextAccessor();
    // Add framework builder.Services.
    _ = builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connString));

    _ = builder.Services.AddDbContext<DataProtectionDbContext>(options => options.UseNpgsql(connString));
    _ = builder.Services.AddDataProtection()
        .SetApplicationName("identity-server")
        .PersistKeysToDbContext<DataProtectionDbContext>();

    if (!string.IsNullOrEmpty(cacheConnection))
    {
        _ = builder.Services.AddStackExchangeRedisCache(options => options.Configuration = cacheConnection);
    }

    var automapperLicense = builder.Configuration.GetValue<string>("AutoMapper:License");

    _ = builder.Services.AddAutoMapper(
        cfg => cfg.LicenseKey = automapperLicense,
        typeof(Program),
        typeof(Spydersoft.Identity.Core.Data.AutoMapper));
    _ = builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            // Use JWT short-form claim types so the ClaimsPrincipal that ASP.NET Identity
            // builds matches what IdentityServer scopes/IdentityResources ask for. Without
            // this, role claims land as ClaimTypes.Role (the long URI form) and are filtered
            // out by Duende's profile service when a scope requests "role". Same story for
            // name/sub. Internal code in this repo already reads via JwtClaimTypes or the
            // Duende User.GetSubjectId() extension, so this is a zero-ripple change here.
            options.ClaimsIdentity.RoleClaimType = Duende.IdentityModel.JwtClaimTypes.Role;
            options.ClaimsIdentity.UserNameClaimType = Duende.IdentityModel.JwtClaimTypes.Name;
            options.ClaimsIdentity.UserIdClaimType = Duende.IdentityModel.JwtClaimTypes.Subject;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    // Add application builder.Services.
    _ = builder.Services.AddTransient<IEmailSender, EmailSender>();

    // Razor Pages host the interactive identity UI (login, consent, device, grants,
    // account management).
    _ = builder.Services.AddRazorPages(options =>
    {
        // Apply the SecurityHeaders (CSP, X-Frame-Options, etc.) to all page responses.
        options.Conventions.ConfigureFilter(new SecurityHeadersAttribute());
        // Pages are anonymous by default; lock down the authenticated areas.
        options.Conventions.AuthorizeFolder("/Manage");
        options.Conventions.AuthorizeFolder("/Consent");
        options.Conventions.AuthorizeFolder("/Device");
        options.Conventions.AuthorizeFolder("/Grants");
    })
    .AddMvcOptions(options =>
    {
        // ViewModels live in Spydersoft.Identity.Core which has Nullable=enable.
        // Without this, every non-nullable reference type property is treated as
        // implicitly [Required], causing legit submissions to fail ModelState
        // validation. Only explicit [Required] attributes should drive validation.
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });

    // Get configured public origin from configuration
    var publicOrigin = builder.Configuration.GetValue<string>("IdentityServer:PublicOrigin");
    var issuerUri = builder.Configuration.GetValue<string>("IdentityServer:IssuerUri");

    // this adds the Configuration Store (clients, resources) and then
    // the Operation Store (codes, tokens, consents)
    var identityServerBuilder = builder.Services.AddIdentityServer(options =>
    {
        // Configure Identity Server's announced issuer. Either IssuerUri or
        // PublicOrigin pins it; without one, the issuer is derived from the
        // incoming request URL which is fragile when the same host is reached
        // via multiple names (e.g. localhost vs 127.0.0.1 in the test harness).
        if (!string.IsNullOrEmpty(issuerUri))
        {
            options.IssuerUri = issuerUri;
        }
        else if (!string.IsNullOrEmpty(publicOrigin))
        {
            options.IssuerUri = publicOrigin;
        }

        // Pin the interactive UI URLs to the Razor Pages routes. These match Duende's
        // defaults but are set explicitly so the page-folder structure and the
        // IdentityServer redirect contract can't drift apart.
        options.UserInteraction.LoginUrl = "/Account/Login";
        options.UserInteraction.LogoutUrl = "/Account/Logout";
        options.UserInteraction.ConsentUrl = "/Consent";
        options.UserInteraction.ErrorUrl = "/Home/Error";
        options.UserInteraction.DeviceVerificationUrl = "/Device";

        Log.Information("IdentityServer configuration - IssuerUri: {IssuerUri}", options.IssuerUri ?? "not set");
    })
    .AddAspNetIdentity<ApplicationUser>()
    .AddConfigurationStore(options => options.ConfigureDbContext = builder =>
            builder.UseNpgsql(connString,
                sql => _ = sql.MigrationsAssembly(migrationsAssembly)))
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = builder =>
            builder.UseNpgsql(connString,
                sql => _ = sql.MigrationsAssembly(migrationsAssembly));

        // this enables automatic token cleanup. this is optional.
        options.EnableTokenCleanup = true;
        options.TokenCleanupInterval = 30;
    });

    var providerSettings = new ProviderOptions();
    builder.Configuration.GetSection(ProviderOptions.SettingsKey).Bind(providerSettings);
    Microsoft.AspNetCore.Authentication.AuthenticationBuilder authBuilder = builder.Services.AddAuthentication();

    if (!string.IsNullOrWhiteSpace(providerSettings.GoogleClientId))
    {
        _ = authBuilder.AddGoogle(option =>
        {
            option.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            option.ClientId = providerSettings.GoogleClientId;
            option.ClientSecret = providerSettings.GoogleClientSecret;
        });
    }

    _ = builder.Services.AddAuthorization();
    _ = builder.Services.AddHealthChecks();

    WebApplication app = builder.Build();
    
    // Configure forwarded headers BEFORE any authentication/authorization middleware
    // This is critical for proper HTTPS scheme detection when behind a TLS termination proxy
    var forwardedHeadersOptions = new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                          ForwardedHeaders.XForwardedProto | 
                          ForwardedHeaders.XForwardedHost,
        // Required for proper scheme detection
        RequireHeaderSymmetry = false,
        ForwardLimit = null
    };
    forwardedHeadersOptions.KnownIPNetworks.Clear();
    forwardedHeadersOptions.KnownProxies.Clear();
    
    _ = app.UseForwardedHeaders(forwardedHeadersOptions);
    
    // Log the scheme being used for debugging (can be removed in production)
    app.Use(async (context, next) =>
    {
        Log.Debug("Request - Scheme: {Scheme}, Host: {Host}, Path: {Path}, Proto Header: {Proto}", 
            context.Request.Scheme, 
            context.Request.Host, 
            context.Request.Path,
            context.Request.Headers["X-Forwarded-Proto"].ToString());
        await next();
    });
    
    // this will do the initial DB population, but we only need to do it once
    // this is just in here as a easy, yet hacky, way to get our DB created/populated
    var dbInitialize = new DatabaseInitializer(app);
    dbInitialize.InitializeDatabase();

    // Migrate the DataProtection DB (identity-server only context — not shared with the API)
    using (var scope = app.Services.CreateScope())
    {
        var dpContext = scope.ServiceProvider.GetRequiredService<DataProtectionDbContext>();
        if ((await dpContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await dpContext.Database.MigrateAsync();
        }
    }

    _ = builder.Environment.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseExceptionHandler("/Home/Error");

    _ = app.UseSpydersoftHealthChecks(healthCheckOptions)
            .UseCookiePolicy()
            .UseStaticFiles()
            .UseRouting()
            .UseAuthentication()
            .UseIdentityServer()
            .UseAuthorization();

    _ = app.MapRazorPages();

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
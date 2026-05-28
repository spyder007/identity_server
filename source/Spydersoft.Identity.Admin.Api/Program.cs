using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Npgsql;

using Scalar.AspNetCore;

using Serilog;

using Spydersoft.Identity.Admin.Api.Data;
using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Data;
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
    Log.Information("Spydersoft.Identity.Admin.Api starting.");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.AddSpydersoftTelemetry(typeof(Program).Assembly,
        new ConfigurationFunctions
        {
            MetricsConfiguration = metrics => metrics.AddNpgsqlInstrumentation()
        })
        .AddSpydersoftSerilog(true);

    AppHealthCheckOptions healthCheckOptions = builder.AddSpydersoftHealthChecks();

    var connString = builder.Configuration.GetConnectionString("IdentityConnection");
    var migrationsAssembly = typeof(ApplicationDbContext).Assembly.GetName().Name;

    var automapperLicense = builder.Configuration.GetValue<string>("AutoMapper:License");

    _ = builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto |
                                   ForwardedHeaders.XForwardedHost;
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
        options.ForwardLimit = null;
    });

    // --- Database ---
    _ = builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connString));

    // ConfigurationDbContext resolves ConfigurationStoreOptions from DI during OnModelCreating;
    // register it explicitly since plain AddDbContext does not.
    _ = builder.Services.AddSingleton(new Duende.IdentityServer.EntityFramework.Options.ConfigurationStoreOptions());
    _ = builder.Services.AddDbContext<Duende.IdentityServer.EntityFramework.DbContexts.ConfigurationDbContext>(options =>
        options.UseNpgsql(connString,
            sql => sql.MigrationsAssembly(migrationsAssembly)));

    // --- Identity (for UserManager/RoleManager) ---
    _ = builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.ClaimsIdentity.RoleClaimType = Duende.IdentityModel.JwtClaimTypes.Role;
            options.ClaimsIdentity.UserNameClaimType = Duende.IdentityModel.JwtClaimTypes.Name;
            options.ClaimsIdentity.UserIdClaimType = Duende.IdentityModel.JwtClaimTypes.Subject;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    // --- AutoMapper ---
    _ = builder.Services.AddAutoMapper(cfg =>
    {
        cfg.LicenseKey = automapperLicense;
        cfg.AddProfile<ApiAutoMapperProfile>();
    });

    // --- Authentication / Authorization ---
    var authorityUrl = builder.Configuration.GetValue<string>("IdentityServer:Authority");

    _ = builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = authorityUrl;
            options.Audience = "identity.admin.api";
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        });

    _ = builder.Services.AddAuthorizationBuilder()
        .AddPolicy(AdminApiPolicies.Read, policy =>
            policy.RequireAuthenticatedUser()
                  .RequireClaim("scope", "identity:admin:read", "identity:admin:write"))
        .AddPolicy(AdminApiPolicies.Write, policy =>
            policy.RequireAuthenticatedUser()
                  .RequireClaim("scope", "identity:admin:write"));

    // --- API Versioning ---
    _ = builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    // --- OpenAPI ---
    _ = builder.Services.AddOpenApi("v1");

    // --- Controllers ---
    _ = builder.Services.AddControllers();

    WebApplication app = builder.Build();

    _ = app.UseForwardedHeaders();

    if (app.Environment.IsDevelopment())
    {
        _ = app.UseDeveloperExceptionPage();
        _ = app.MapOpenApi();
        _ = app.MapScalarApiReference(options =>
        {
            options.WithTitle("Identity Admin API")
                   .WithTheme(Scalar.AspNetCore.ScalarTheme.Purple)
                   .WithDefaultHttpClient(Scalar.AspNetCore.ScalarTarget.CSharp, Scalar.AspNetCore.ScalarClient.HttpClient);
        });
    }

    _ = app.UseSpydersoftHealthChecks(healthCheckOptions)
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization();

    _ = app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Spydersoft.Identity.Admin.Api failed to start.");
}
finally
{
    Log.Information("Spydersoft.Identity.Admin.Api shut down complete.");
    await Log.CloseAndFlushAsync();
}

/// <summary>Authorization policy names for the Admin API.</summary>
public static class AdminApiPolicies
{
    /// <summary>Policy for read-only operations (GET). Accepts read or write scope.</summary>
    public const string Read = "identity:admin:read";

    /// <summary>Policy for mutating operations (POST, PUT, DELETE). Requires write scope.</summary>
    public const string Write = "identity:admin:write";
}

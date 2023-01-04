using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

using Serilog;

using spydersoft.Identity;

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

    _ = builder.Host.UseSerilog((context, services, configuration) => _ = configuration.ReadFrom.Configuration(context.Configuration));

    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication app = builder.Build();
    startup.Configure(app, app.Environment);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "identityServer failed to start.");
}
finally
{
    Log.Information("identityServer shut down complete");
    Log.CloseAndFlush();
}
﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using spydersoft.Identity;

var baseConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(baseConfig)
    .CreateBootstrapLogger();

try
{
    Log.Information("identityServer starting.");
    var builder = WebApplication.CreateBuilder(args);
    var config = builder.Configuration;

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    var app = builder.Build();
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
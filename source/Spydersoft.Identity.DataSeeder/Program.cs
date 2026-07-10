using Duende.IdentityModel;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Storage;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Spydersoft.Identity.Core.Models.Identity;
using Spydersoft.Identity.Data;
using Spydersoft.Identity.DataSeeder.Seeding;

var builder = Host.CreateApplicationBuilder(args);

var connString = builder.Configuration.GetConnectionString("IdentityConnection")
    ?? "Host=localhost;Port=7010;Database=identity;Username=postgres;Password=postgres";

var migrationsAssembly = typeof(ApplicationDbContext).Assembly.GetName().Name!;

// Suppress the EF 10 PendingModelChangesWarning for the seeder only: the
// migrations were generated under an earlier EF version, so the in-memory
// model snapshot can drift slightly from the compiled model even though the
// generated DDL is still correct. The seeder is a fresh-DB convenience tool
// and we want it to keep working across EF bumps; the main app keeps strict
// validation.
void ConfigureNpgsql(DbContextOptionsBuilder o) => o
    .UseNpgsql(connString, sql => sql.MigrationsAssembly(migrationsAssembly))
    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

builder.Services.AddDbContext<ApplicationDbContext>(ConfigureNpgsql);

// Duende's Configuration/Operational DbContexts need their options objects
// (ConfigurationStoreOptions/OperationalStoreOptions) registered too; the
// AddConfigurationDbContext / AddOperationalDbContext extensions wire both
// the context and the options.
builder.Services.AddConfigurationDbContext(o => o.ConfigureDbContext = ConfigureNpgsql);
builder.Services.AddOperationalDbContext(o => o.ConfigureDbContext = ConfigureNpgsql);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
        options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
        options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

using var host = builder.Build();
await DatabaseSeeder.RunAsync(host.Services);

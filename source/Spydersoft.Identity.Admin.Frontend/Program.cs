using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Logging;

using OidcProxy.Net.ModuleInitializers;
using OidcProxy.Net.OpenIdConnect;

using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSpydersoftTelemetry(typeof(Program).Assembly);
builder.AddSpydersoftSerilog(true);
AppHealthCheckOptions healthCheckOptions = builder.AddSpydersoftHealthChecks();

var config = builder.Configuration
    .GetSection("OidcProxySettings")
    .Get<OidcProxyConfig>();

if (config != null)
{
    builder.Services.AddOidcProxy(config, options => options.AllowAnonymousAccess = true);
}
else
{
    throw new InvalidOperationException("OidcProxySettings configuration section is missing or invalid.");
}

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAuthenticatedUserPolicy", policy => policy.RequireAuthenticatedUser())
    .AddPolicy("RequireAdminRolePolicy", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("role", "authentication.admin"));

builder.Services.PostConfigureAll<CookieAuthenticationOptions>(options =>
{
    options.AccessDeniedPath = "/forbidden.html";
});

builder.Services.AddControllers();
builder.Services.AddHttpClient("diag");

var app = builder.Build();

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                        ForwardedHeaders.XForwardedProto |
                        ForwardedHeaders.XForwardedHost,
    ForwardLimit = null,
};
forwardedHeadersOptions.KnownIPNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

app.UseRouting();
app.UseOidcProxy();
app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
    IdentityModelEventSource.LogCompleteSecurityArtifact = true;
}

app.MapControllers();
app.UseSpydersoftHealthChecks(healthCheckOptions);

if (app.Environment.IsDevelopment())
{
    app.MapGet("/.diag/session", async (HttpContext ctx) =>
    {
        static object? TryDecodeJwt(string val)
        {
            var parts = val.Split('.');
            if (parts.Length != 3) return null;
            try
            {
                var p = parts[1].Replace('-', '+').Replace('_', '/');
                p = p.PadRight((p.Length + 3) / 4 * 4, '=');
                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(p));
                return System.Text.Json.JsonDocument.Parse(json).RootElement.Clone();
            }
            catch { return null; }
        }

        await ctx.Session.LoadAsync();
        var data = new Dictionary<string, object?>();
        foreach (var key in ctx.Session.Keys)
        {
            var val = ctx.Session.GetString(key);
            if (val is null) { data[key] = null; continue; }
            var claims = (key.EndsWith("token_key") || key.EndsWith("id_token_key"))
                ? TryDecodeJwt(val)
                : null;
            data[key] = new
            {
                length = val.Length,
                preview = val.Length > 60 ? val[..60] + "..." : val,
                claims,
            };
        }
        return Results.Json(new
        {
            sessionId = ctx.Session.Id,
            nowUtc = DateTimeOffset.UtcNow.ToString("O"),
            nowEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            keys = data,
        });
    });

    app.MapGet("/.diag/test-api", async (HttpContext ctx, IHttpClientFactory factory) =>
    {
        await ctx.Session.LoadAsync();
        var token = ctx.Session.GetString("OidcProxy.Net.IdentityProviders.IIdentityProvider-token_key");
        if (string.IsNullOrEmpty(token))
            return Results.Json(new { error = "no access token in session" });

        using var client = factory.CreateClient("diag");
        var req = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7030/api/v1/clients");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        using var resp = await client.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        var headers = resp.Headers
            .Concat(resp.Content.Headers)
            .ToDictionary(h => h.Key, h => string.Join(",", h.Value));
        return Results.Json(new
        {
            status = (int)resp.StatusCode,
            statusText = resp.StatusCode.ToString(),
            headers,
            body = body.Length > 500 ? body[..500] + "..." : body,
        });
    });
}

app.MapFallbackToFile("/index.html").RequireAuthorization("RequireAdminRolePolicy");

await app.RunAsync();

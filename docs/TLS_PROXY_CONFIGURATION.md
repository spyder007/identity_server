# TLS Termination Proxy Configuration

## Overview

When hosting this Identity Server behind a TLS termination proxy (such as Nginx, Traefik, AWS ALB, Azure Application Gateway, etc.), additional configuration is required to ensure that the application correctly generates HTTPS URLs instead of HTTP URLs.

## The Problem

TLS termination proxies handle SSL/TLS encryption, meaning:
- External clients connect to the proxy using HTTPS
- The proxy decrypts the traffic
- The proxy forwards the request to your application using HTTP
- Without proper configuration, your application thinks it's running on HTTP and generates HTTP URLs

This causes issues with:
- OAuth/OIDC redirect URIs
- Discovery document URLs
- Token endpoint URLs
- Other Identity Server endpoints

## The Solution

The application has been configured to handle this scenario through:

1. **Forwarded Headers Middleware**: Respects `X-Forwarded-*` headers from the proxy
2. **Identity Server IssuerUri Configuration**: Explicitly sets the public-facing HTTPS URL

## Configuration Steps

### 1. Update appsettings.json or User Secrets

Add the following configuration to your `appsettings.json` or user secrets:

```json
{
  "IdentityServer": {
    "PublicOrigin": "https://your-domain.com",
    "IssuerUri": "https://your-domain.com"
  }
}
```

**Important Notes:**
- `PublicOrigin`: The public-facing HTTPS URL of your identity server
- `IssuerUri`: The issuer identifier used in tokens (usually same as PublicOrigin)
- Both should use HTTPS scheme
- Do not include trailing slashes
- Use your actual domain name

### 2. Configure Your Proxy

Your TLS termination proxy must be configured to forward the appropriate headers:

#### Nginx Example

```nginx
location / {
    proxy_pass http://your-app:5000;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    proxy_set_header X-Forwarded-Host $host;
}
```

#### Traefik Example

Traefik automatically adds forwarded headers when configured as an HTTPS entrypoint.

```yaml
entryPoints:
  web:
    address: ":80"
  websecure:
    address: ":443"
    http:
      tls:
        certResolver: letsencrypt
```

#### Apache Example

```apache
ProxyPreserveHost On
RequestHeader set X-Forwarded-Proto https
RequestHeader set X-Forwarded-Host %{HTTP_HOST}e
ProxyPass / http://your-app:5000/
ProxyPassReverse / http://your-app:5000/
```

### 3. Update Client Redirect URIs

Ensure all registered clients in your Identity Server have redirect URIs that use HTTPS:

```csharp
RedirectUris = { "https://your-client-app.com/signin-oidc" },
PostLogoutRedirectUris = { "https://your-client-app.com/signout-callback-oidc" }
```

## Verification

### 1. Check Discovery Document

Navigate to `https://your-domain.com/.well-known/openid-configuration`

Verify that all URLs in the response use HTTPS:

```json
{
  "issuer": "https://your-domain.com",
  "authorization_endpoint": "https://your-domain.com/connect/authorize",
  "token_endpoint": "https://your-domain.com/connect/token",
  ...
}
```

### 2. Check Application Logs

When the application starts, you should see log entries like:

```
IdentityServer IssuerUri configured: https://your-domain.com
```

### 3. Monitor Request Logs

In development mode, the application logs each request with scheme information:

```
Request - Scheme: https, Host: your-domain.com, Path: /, Proto Header: https
```

If you see `Scheme: http` but `Proto Header: https`, then the forwarded headers are being sent but not processed correctly.

## Troubleshooting

### URLs Still Using HTTP

**Possible Causes:**
1. `IdentityServer:PublicOrigin` not set in configuration
2. Proxy not sending `X-Forwarded-Proto` header
3. Proxy not sending `X-Forwarded-Host` header
4. Application restarted but configuration not reloaded

**Solutions:**
1. Verify configuration in appsettings.json or user secrets
2. Check proxy configuration and ensure forwarded headers are being sent
3. Restart the application after configuration changes
4. Check application logs for configuration values

### Mixed Content Warnings

If you see mixed content warnings (HTTPS page loading HTTP resources):

1. Verify all static file references use relative URLs or HTTPS
2. Check that `UseForwardedHeaders` is called before other middleware
3. Ensure CSS/JS bundles are served over HTTPS

### Discovery Document Issues

If clients can't discover endpoints:

1. Verify the discovery document URL is accessible: `https://your-domain.com/.well-known/openid-configuration`
2. Check that the `issuer` field matches your configured `IssuerUri`
3. Verify all endpoint URLs use HTTPS

## Security Considerations

### Trust Proxy Headers

The current configuration trusts all proxies by clearing `KnownNetworks` and `KnownProxies`. In production, you should:

1. **Restrict to known proxies**: Add your proxy's IP address to `KnownProxies`
2. **Restrict to known networks**: Add your proxy's network to `KnownNetworks`

Example secure configuration:

```csharp
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                               ForwardedHeaders.XForwardedProto | 
                               ForwardedHeaders.XForwardedHost;
    
    // Only trust specific proxy IP
    options.KnownProxies.Add(IPAddress.Parse("10.0.0.1"));
    
    // Or trust specific network range
    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 24));
    
    options.ForwardLimit = 1; // Only accept one level of proxy
    options.RequireHeaderSymmetry = true; // Require all or none of the forwarded headers
});
```

### HTTPS Enforcement

Consider adding HTTPS redirection for direct access:

```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
```

**Note:** Only enable this if your application is directly exposed to the internet. If it's behind a proxy that terminates TLS, do not enable HTTPS redirection in the application.

## Environment-Specific Configuration

### Development

```json
{
  "IdentityServer": {
    "PublicOrigin": "",
    "IssuerUri": ""
  }
}
```

Leave empty for local development without a proxy.

### Production

```json
{
  "IdentityServer": {
    "PublicOrigin": "https://identity.production.com",
    "IssuerUri": "https://identity.production.com"
  }
}
```

Set to your production domain.

### Using Environment Variables

You can also set these via environment variables:

```bash
export IdentityServer__PublicOrigin="https://identity.production.com"
export IdentityServer__IssuerUri="https://identity.production.com"
```

Or in Docker:

```dockerfile
ENV IdentityServer__PublicOrigin=https://identity.production.com
ENV IdentityServer__IssuerUri=https://identity.production.com
```

## Additional Resources

- [Duende IdentityServer - Proxy and Load Balancer Support](https://docs.duendesoftware.com/identityserver/v6/deployment/proxies/)
- [ASP.NET Core - Configure ASP.NET Core to work with proxy servers](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer)
- [Forwarded Headers Middleware](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#forwarded-headers-middleware-options)

## Support

If you continue to experience issues:

1. Enable debug logging to see all request details
2. Use browser developer tools to inspect actual URLs being used
3. Check proxy logs for header forwarding
4. Verify network connectivity between proxy and application

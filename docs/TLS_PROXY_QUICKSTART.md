# Quick Start: Configuring for TLS Termination Proxy

## Minimum Required Configuration

### 1. Add to appsettings.json (or User Secrets)

```json
{
  "IdentityServer": {
    "PublicOrigin": "https://your-domain.com",
    "IssuerUri": "https://your-domain.com"
  }
}
```

Replace `your-domain.com` with your actual domain.

### 2. Configure Your Proxy

Ensure your proxy forwards these headers:
- `X-Forwarded-Proto` (should be "https")
- `X-Forwarded-Host` (your domain name)
- `X-Forwarded-For` (client IP, optional)

### 3. Restart the Application

The application will now generate HTTPS URLs for all Identity Server endpoints.

## Verification

Check the discovery document:
```
https://your-domain.com/.well-known/openid-configuration
```

All URLs should use `https://` scheme.

## Common Proxies Quick Reference

### Nginx
```nginx
proxy_set_header X-Forwarded-Proto $scheme;
proxy_set_header X-Forwarded-Host $host;
```

### Traefik
Automatically handled when using HTTPS entrypoint.

### Apache
```apache
RequestHeader set X-Forwarded-Proto https
RequestHeader set X-Forwarded-Host %{HTTP_HOST}e
```

### HAProxy
```
http-request set-header X-Forwarded-Proto https if { ssl_fc }
http-request set-header X-Forwarded-Host %[req.hdr(Host)]
```

### AWS ALB / Azure Application Gateway
Automatically forwarded. No additional configuration needed.

## That's It!

Your Identity Server is now properly configured for TLS termination proxy scenarios.

For detailed troubleshooting and security considerations, see [TLS_PROXY_CONFIGURATION.md](TLS_PROXY_CONFIGURATION.md).

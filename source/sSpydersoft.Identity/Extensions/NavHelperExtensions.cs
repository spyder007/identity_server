using Microsoft.AspNetCore.Http;

namespace Spydersoft.Identity.Extensions
{
    public static class NavHelperExtensions
    {
        public static bool IsISAdminRoute(this HttpContext context)
        {
            if (!context.Request.Path.HasValue)
            {
                return false;
            }

            var path = context.Request.Path.Value?.Trim('/').ToLower();
            return path is "clients" or "identityresources" or "apiresources" or "scopes";
        }

        public static bool IsUserAdminRoute(this HttpContext context)
        {
            if (!context.Request.Path.HasValue)
            {
                return false;
            }

            var path = context.Request.Path.Value?.Trim('/').ToLower();
            return path is "users" or "userroles";
        }

        public static bool IsSpydersoftRoute(this HttpContext context)
        {
            if (!context.Request.Path.HasValue)
            {
                return false;
            }

            var path = context.Request.Path.Value?.Trim('/').ToLower();
            return path is "home/contact" or "home/about";
        }
    }
}
using Microsoft.AspNetCore.Http;

namespace Spydersoft.Identity.Extensions
{
    /// <summary>
    /// Class NavHelperExtensions.
    /// </summary>
    public static class NavHelperExtensions
    {
        /// <summary>
        /// Determines whether [is is admin route] [the specified context].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if [is is admin route] [the specified context]; otherwise, <c>false</c>.</returns>
        public static bool IsISAdminRoute(this HttpContext context)
        {
            if (!context.Request.Path.HasValue)
            {
                return false;
            }

            var path = context.Request.Path.Value?.Trim('/').ToLower();
            return path is "clients" or "identityresources" or "apiresources" or "scopes";
        }

        /// <summary>
        /// Determines whether [is user admin route] [the specified context].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if [is user admin route] [the specified context]; otherwise, <c>false</c>.</returns>
        public static bool IsUserAdminRoute(this HttpContext context)
        {
            if (!context.Request.Path.HasValue)
            {
                return false;
            }

            var path = context.Request.Path.Value?.Trim('/').ToLower();
            return path is "users" or "userroles";
        }

        /// <summary>
        /// Determines whether [is spydersoft route] [the specified context].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if [is spydersoft route] [the specified context]; otherwise, <c>false</c>.</returns>
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
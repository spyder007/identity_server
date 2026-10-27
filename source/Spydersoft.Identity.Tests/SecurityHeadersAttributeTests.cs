using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;

using Spydersoft.Identity.Attributes;

namespace Spydersoft.identity.tests
{
    public class SecurityHeadersAttributeTests
    {
        private static ResultExecutingContext CreateContext(IActionResult result, HttpContext httpContext)
        {
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ResultExecutingContext(actionContext, [], result, new object());
        }

        [Test]
        public void OnResultExecuting_PageResult_AddsSecurityHeaders()
        {
            var httpContext = new DefaultHttpContext();
            var context = CreateContext(new PageResult(), httpContext);

            new SecurityHeadersAttribute().OnResultExecuting(context);

            var headers = httpContext.Response.Headers;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(headers.XContentTypeOptions.ToString(), Is.EqualTo("nosniff"));
                Assert.That(headers.XFrameOptions.ToString(), Is.EqualTo("SAMEORIGIN"));
                Assert.That(headers["Referrer-Policy"].ToString(), Is.EqualTo("no-referrer"));
                Assert.That(headers.ContentSecurityPolicy.ToString(), Does.Contain("allow-popups-to-escape-sandbox"));
                Assert.That(headers["X-Content-Security-Policy"].ToString(), Does.Contain("allow-popups"));
            }
        }

        [Test]
        public void OnResultExecuting_ExistingHeaders_AreNotOverwritten()
        {
            var httpContext = new DefaultHttpContext();
            var headers = httpContext.Response.Headers;
            headers.XContentTypeOptions = "custom";
            headers.XFrameOptions = "custom";
            headers.ContentSecurityPolicy = "custom";
            headers["X-Content-Security-Policy"] = "custom";
            headers["Referrer-Policy"] = "custom";
            var context = CreateContext(new PageResult(), httpContext);

            new SecurityHeadersAttribute().OnResultExecuting(context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(headers.XContentTypeOptions.ToString(), Is.EqualTo("custom"));
                Assert.That(headers.XFrameOptions.ToString(), Is.EqualTo("custom"));
                Assert.That(headers.ContentSecurityPolicy.ToString(), Is.EqualTo("custom"));
                Assert.That(headers["X-Content-Security-Policy"].ToString(), Is.EqualTo("custom"));
                Assert.That(headers["Referrer-Policy"].ToString(), Is.EqualTo("custom"));
            }
        }

        [Test]
        public void OnResultExecuting_NonViewResult_AddsNoHeaders()
        {
            var httpContext = new DefaultHttpContext();
            var context = CreateContext(new OkResult(), httpContext);

            new SecurityHeadersAttribute().OnResultExecuting(context);

            Assert.That(httpContext.Response.Headers, Is.Empty);
        }
    }
}

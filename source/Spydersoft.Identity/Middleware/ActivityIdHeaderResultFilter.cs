using System.Diagnostics;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Spydersoft.Identity.Middleware
{
    public class ActivityIdHeaderResultFilter
    : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            context
                .HttpContext
                .Response
                .Headers
                .Add("X-ActivityId", Activity.Current?.Id);

        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
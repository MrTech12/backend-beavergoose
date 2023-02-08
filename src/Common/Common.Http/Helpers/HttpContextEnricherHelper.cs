using Common.Http.Models;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Common.Http.Helpers
{
    public static class HttpContextEnricherHelper
    {
        public static void HttpRequestEnricher(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var httpContextInfo = new HttpContextInfo
            {
                Protocol = httpContext.Request.Protocol,
                Scheme = httpContext.Request.Scheme,
                Host = httpContext.Request.Host.ToString(),
            };

            diagnosticContext.Set("HttpContext", httpContextInfo, true);
        }
    }
}

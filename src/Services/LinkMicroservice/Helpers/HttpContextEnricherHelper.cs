using LinkMicroservice.Models;
using Serilog;

namespace LinkMicroservice.Helpers
{
    public static class HttpContextEnricherHelper
    {
		internal static void HttpRequestEnricher(IDiagnosticContext diagnosticContext, HttpContext httpContext)
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

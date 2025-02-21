using Serilog.Context;

namespace Bookly.Api.Middleware
{
    public class RequestContextLoggingMiddleware
    {
        private const string CorrelationIdHeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public RequestContextLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("CorrelationId", GetCorrelationId(context)))
            {
                return _next(context);  
            }
        }

        private static string GetCorrelationId(HttpContext context)
        {
            context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationid);

            return correlationid.FirstOrDefault() ?? context.TraceIdentifier;
        }
    }
}

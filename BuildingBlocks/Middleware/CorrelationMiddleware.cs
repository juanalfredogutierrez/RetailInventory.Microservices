using BuildingBlocks.Correlation;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Middleware.Correlation;

public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Trace-Id", out var traceId))
        {
            CorrelationContext.TraceId = traceId!;
        }
        else
        {
            CorrelationContext.TraceId = Guid.NewGuid().ToString();
        }

        context.Response.Headers["X-Trace-Id"] = CorrelationContext.TraceId;

        await _next(context);
    }
}
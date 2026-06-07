
using BuildingBlocks.Correlation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
       TRequest request,
       RequestHandlerDelegate<TResponse> next,
       CancellationToken cancellationToken)
    {
        var traceId = CorrelationContext.TraceId;

        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "[START] {Request} | TraceId: {TraceId}",
            typeof(TRequest).Name,
            traceId);

        var response = await next();

        stopwatch.Stop();

        _logger.LogInformation(
            "[END] {Request} | TraceId: {TraceId} | {Elapsed}ms",
            typeof(TRequest).Name,
            traceId,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}   
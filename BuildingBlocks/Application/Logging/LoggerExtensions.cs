
using BuildingBlocks.Correlation;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Logging;

public static class LoggerExtensions
{
    public static void LogWithTrace(
        this ILogger logger,
        LogLevel level,
        string message)
    {
        logger.Log(level,
            "[TraceId:{TraceId}] {Message}",
            CorrelationContext.TraceId,
            message);
    }
}
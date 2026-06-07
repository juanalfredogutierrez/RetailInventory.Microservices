using BuildingBlocks.Correlation;
using Microsoft.Extensions.Logging;

public static class LoggerExtensions
{
    public static void LogBusiness(
        this ILogger logger,
        string message)
    {
        logger.LogInformation(
            "[BUSINESS] [TraceId:{TraceId}] {Message}",
            CorrelationContext.TraceId,
            message);
    }

    public static void LogIntegration(
        this ILogger logger,
        string message)
    {
        logger.LogInformation(
            "[INTEGRATION] [TraceId:{TraceId}] {Message}",
            CorrelationContext.TraceId,
            message);
    }
}
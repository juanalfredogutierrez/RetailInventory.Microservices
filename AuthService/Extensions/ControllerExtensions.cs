
namespace AuthService.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult ToActionResult<T>(this ControllerBase controller,Result<T> result)
        {
            if (result.IsFailure)
            {
                return controller.BadRequest(new
                {
                    errors = result.Errors,
                    traceId = CorrelationContext.TraceId
                });
            }

            return controller.Ok(new
            {
                data = result.Value,
                traceId = CorrelationContext.TraceId
            });
        }
    }
}

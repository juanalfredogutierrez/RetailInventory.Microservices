namespace InventarioService.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UseApi(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseMiddleware<CorrelationMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            app.MapControllers();

            return app;
        }
    }
}

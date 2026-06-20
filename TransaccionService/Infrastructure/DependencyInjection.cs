
namespace TransaccionService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                     IConfiguration configuration)
        {
            services.AddDbContext<TransaccionDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddHttpClient("ProductoApi", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["Services:ProductoApi"]!);
            });

            services.AddHttpClient("InventarioApi", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["Services:InventarioApi"]!);
            });

            services.AddHostedService<OutboxPublisherWorker>();
            services.AddRabbitMq(configuration);

            return services;
        }
    }
}

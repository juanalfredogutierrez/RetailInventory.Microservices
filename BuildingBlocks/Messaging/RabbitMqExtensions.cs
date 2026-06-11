using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace BuildingBlocks.Messaging;

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services,string hostName = "localhost")
    {
        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName
            };

            return factory.CreateConnectionAsync()
                          .GetAwaiter()
                          .GetResult();
        });

        services.AddScoped<IChannel>(sp =>
        {
            var connection = sp.GetRequiredService<IConnection>();

            return connection.CreateChannelAsync()
                             .GetAwaiter()
                             .GetResult();
        });

        services.AddScoped<IMessagePublisher,RabbitMqPublisher>();

        return services;
    }
}
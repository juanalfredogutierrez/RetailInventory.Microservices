using BuildingBlocks.Correlation;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BuildingBlocks.Messaging;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IChannel _channel;

    public RabbitMqPublisher(IChannel channel)
    {
        _channel = channel;
    }

    public async Task PublishAsync<T>(
        string queue,
        T message)
    {
        await _channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var traceId = CorrelationContext.TraceId;

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object?>
            {
                ["X-Trace-Id"] = traceId
            }
        };

        var json = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queue,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}

using BuildingBlocks.Correlation;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace TransaccionService.Infrastructure.Messaging;

public class RabbitMqPublisher
{
    private readonly IModel _channel;

    public RabbitMqPublisher(IModel channel)
    {
        _channel = channel;
    }

    public void Publish<T>(string queue, T message)
    {
        var traceId = CorrelationContext.TraceId;

        var props = _channel.CreateBasicProperties();
        props.Headers = new Dictionary<string, object>
        {
            { "X-Trace-Id", traceId }
        };

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: "",
            routingKey: queue,
            basicProperties: props,
            body: body
        );
    }
}
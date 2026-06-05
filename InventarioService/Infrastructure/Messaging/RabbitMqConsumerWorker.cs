using InventarioService.Application.Commands.RegistrarEntrada;
using InventarioService.Application.Commands.RegistrarSalida;
using InventarioService.Domain.Events;
using InventarioService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace InventarioService.Infrastructure.Messaging;

public class RabbitMqConsumerWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMqConsumerWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare("compra.registrada", true, false);
        _channel.QueueDeclare("venta.registrada", true, false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (_, eventArgs) =>
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (eventArgs.RoutingKey == "compra.registrada")
            {
                var evt = JsonSerializer.Deserialize<CompraRegistradaEvent>(message);

                if (await ExisteEvento(db, evt.EventId)) return;

                await GuardarEvento(db, evt.EventId, "CompraRegistradaEvent");

 
                foreach (var item in evt.Items)
                {
                    await mediator.Send(new RegistrarEntradaCommand(
                        item.ProductoId,
                        item.Cantidad,
                        evt.NumeroCompra
                    ));
                }
            }

            if (eventArgs.RoutingKey == "venta.registrada")
            {
                var evt = JsonSerializer.Deserialize<VentaRegistradaEvent>(message);

                if (await ExisteEvento(db, evt.EventId)) return;

                await GuardarEvento(db, evt.EventId, "VentaRegistradaEvent");

                foreach (var item in evt.Items)
                {
                    await mediator.Send(new RegistrarSalidaCommand(
                        item.ProductoId,
                        item.Cantidad
                    ));
                }
            }
        };

        _channel.BasicConsume("compra.registrada", true, consumer);
        _channel.BasicConsume("venta.registrada", true, consumer);

        return Task.CompletedTask;
    }

    private static async Task<bool> ExisteEvento(InventarioDbContext db, Guid eventId)
    {
        return await db.EventosProcesados.AnyAsync(x => x.EventoId == eventId);
    }

    private static async Task GuardarEvento(InventarioDbContext db, Guid eventId, string name)
    {
        db.EventosProcesados.Add(new Domain.Entities.EventoProcesado
        {
            Id = Guid.NewGuid(),
            EventoId = eventId,
            NombreEvento = name,
            FechaProcesamiento = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }
}
using MediatR;
using TransaccionService.Domain.Entities;
using TransaccionService.Domain.Events;
using TransaccionService.Infrastructure.Messaging;
using TransaccionService.Infrastructure.Persistence;

namespace TransaccionService.Application.Commands.CreateCompra;

public class CreateCompraHandler : IRequestHandler<CreateCompraCommand, Guid>
{
    private readonly TransaccionDbContext _context;
    private readonly RabbitMqPublisher _publisher;
    private readonly ILogger<CreateCompraHandler> _logger;

    public CreateCompraHandler(
        TransaccionDbContext context,
        RabbitMqPublisher publisher,
        ILogger<CreateCompraHandler> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateCompraCommand request, CancellationToken cancellationToken)
    {
        var compra = new Compra
        {
            NumeroCompra = $"COM-{Guid.NewGuid().ToString()[..6]}",
            FechaCompra = DateTime.UtcNow,
            Estado = "CREADA",
            Observacion = request.Observacion,
            TotalCompra = request.Detalles.Sum(x => x.Cantidad * x.PrecioUnitario),
            Detalles = request.Detalles.Select(x => new DetalleCompra
            {
                ProductoId = x.ProductoId,
                Cantidad = x.Cantidad,
                PrecioUnitario = x.PrecioUnitario,
                Subtotal = x.Cantidad * x.PrecioUnitario
            }).ToList()
        };

        _context.Compras.Add(compra);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogBusiness($"Compra {compra.NumeroCompra} registrada");

        await _publisher.PublishAsync("compra.registrada", new CompraRegistradaEvent
        {

            NumeroCompra = compra.NumeroCompra,
            Items = compra.Detalles.Select(x =>
                new CompraRegistradaItemEvent
                {
                    ProductoId = x.ProductoId,
                    Cantidad = x.Cantidad
                })
        .ToList()
        });

        _logger.LogIntegration("Evento compra.registrada enviado a RabbitMQ");
        return compra.Uid;
    }
}
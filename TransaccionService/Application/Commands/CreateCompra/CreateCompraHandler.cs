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

    public CreateCompraHandler(
        TransaccionDbContext context,
        RabbitMqPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
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
        try
        {
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
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }

        return compra.Uid;
    }
}
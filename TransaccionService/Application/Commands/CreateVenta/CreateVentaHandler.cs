using MediatR;
using System.Net.Http;
using TransaccionService.Domain.Entities;
using TransaccionService.Domain.Events;
using TransaccionService.Infrastructure.Messaging;
using TransaccionService.Infrastructure.Persistence;

namespace TransaccionService.Application.Commands.CreateVenta;

public class CreateVentaHandler : IRequestHandler<CreateVentaCommand, Guid>
{
    private readonly TransaccionDbContext _context;
    private readonly RabbitMqPublisher _publisher;
    private readonly IHttpClientFactory _httpClientFactory;
    public CreateVentaHandler(
        TransaccionDbContext context,
        RabbitMqPublisher publisher,
        IHttpClientFactory httpClientFactory    )
    {
        _context = context;
        _publisher = publisher;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Guid> Handle(CreateVentaCommand request, CancellationToken cancellationToken)
    {

        foreach (var detalle in request.Detalles)
        {
            var stockDisponible =
                await ObtenerStockAsync(detalle.ProductoId);

            if (stockDisponible < detalle.Cantidad)
            {
                throw new Exception(
                    $"Stock insuficiente para el producto {detalle.ProductoId}. " +
                    $"Disponible: {stockDisponible}, Solicitado: {detalle.Cantidad}");
            }
        }

        var venta = new Venta
        {
            NumeroVenta = $"VEN-{Guid.NewGuid().ToString()[..6]}",
            FechaVenta = DateTime.Now,
            Estado = "CREADA",
            Observacion = request.Observacion,
            TotalVenta = request.Detalles.Sum(x => x.Cantidad * x.PrecioUnitario),
            Detalles = request.Detalles.Select(x => new DetalleVenta
            {
                ProductoId = x.ProductoId,
                Cantidad = x.Cantidad,
                PrecioUnitario = x.PrecioUnitario,
                Subtotal = x.Cantidad * x.PrecioUnitario
            }).ToList()
        };

        _context.Ventas.Add(venta);
        await _context.SaveChangesAsync(cancellationToken);

        await _publisher.PublishAsync(
        "venta.registrada",
        new VentaRegistradaEvent
        {
            EventId = Guid.NewGuid(),
            NumeroVenta = venta.NumeroVenta,

            Items = venta.Detalles.Select(x =>
                new VentaRegistradaItemEvent
        {
            ProductoId = x.ProductoId,
            Cantidad = x.Cantidad
        })
        .ToList()
        });

        return venta.Uid;
    }

    private async Task<int> ObtenerStockAsync(int productoId)
    {
        var client = _httpClientFactory.CreateClient("InventarioApi");

        var response = await client.GetAsync(
            $"api/inventario/stock/{productoId}");

        response.EnsureSuccessStatusCode();

        var contenido = await response.Content.ReadAsStringAsync();

        return int.Parse(contenido);
    }
}
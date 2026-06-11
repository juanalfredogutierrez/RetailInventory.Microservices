using BuildingBlocks.Application;
using BuildingBlocks.Messaging;
using MediatR;
using TransaccionService.Application.DTOs;
using TransaccionService.Domain.Entities;
using TransaccionService.Domain.Errors;
using TransaccionService.Domain.Events;
using TransaccionService.Infrastructure.Persistence;

namespace TransaccionService.Application.Commands.CreateVenta;

public class CreateVentaHandler : IRequestHandler<CreateVentaCommand, Result<Guid>>
{
    private readonly TransaccionDbContext _context;
    private readonly IMessagePublisher _publisher;
    private readonly IHttpClientFactory _httpClientFactory;
    public CreateVentaHandler(
        TransaccionDbContext context,
        IMessagePublisher publisher,
        IHttpClientFactory httpClientFactory    )
    {
        _context = context;
        _publisher = publisher;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Result<Guid>> Handle(
      CreateVentaCommand request,
      CancellationToken cancellationToken)
    {
        if (!request.Detalles.Any())
        {
            return Result<Guid>.Failure(
                VentaErrors.SinItems);
        }

        var errors = new List<Error>();

        foreach (var detalle in request.Detalles)
        {
            var stockDisponible =
                await ObtenerStockAsync(detalle.ProductoId);

            if (stockDisponible < detalle.Cantidad)
            {
                errors.Add(
                    VentaErrors.StockInsuficiente(
                        detalle.ProductoId,
                        stockDisponible,
                        detalle.Cantidad));
            }
        }

        if (errors.Count > 0)
        {
            return Result<Guid>.Failure(
                errors.ToArray());
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

        var result =
            await client.GetFromJsonAsync<ApiResponse<int>>(
                $"api/inventario/stock/{productoId}");

        return result?.Data ?? 0;
    }
}
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
    private readonly ILogger<CreateVentaHandler> _logger;
    private readonly TransaccionDbContext _context;
    private readonly IMessagePublisher _publisher;
    private readonly IHttpClientFactory _httpClientFactory;

    public CreateVentaHandler(
        TransaccionDbContext context,
        IMessagePublisher publisher,
        IHttpClientFactory httpClientFactory,
        ILogger<CreateVentaHandler> logger)
    {
        _context = context;
        _publisher = publisher;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
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

        if (request.Detalles.Any(x => x.ProductoId <= 0))
        {
            return Result<Guid>.Failure(
                VentaErrors.ProductoInvalido);
        }

        if (request.Detalles.Any(x => x.Cantidad <= 0))
        {
            return Result<Guid>.Failure(
                VentaErrors.CantidadInvalida);
        }

        if (request.Detalles.Any(x => x.PrecioUnitario <= 0))
        {
            return Result<Guid>.Failure(
                VentaErrors.PrecioInvalido);
        }

        _logger.LogBusiness($"Registrando venta con {request.Detalles.Count} productos");

        var validacionesStock = request.Detalles.Select(async detalle =>
        {
            _logger.LogBusiness($"Consultando stock del producto {detalle.ProductoId}");

            var stockDisponible =
                await ObtenerStockAsync(detalle.ProductoId);

            if (stockDisponible < detalle.Cantidad)
            {
                _logger.LogWarning(
                    "Stock insuficiente para producto {ProductoId}. Disponible: {Disponible}, Solicitado: {Solicitado}",
                    detalle.ProductoId,
                    stockDisponible,
                    detalle.Cantidad);

                return VentaErrors.StockInsuficiente(
                    detalle.ProductoId,
                    stockDisponible,
                    detalle.Cantidad);
            }

            return null;
        });

        var errores = await Task.WhenAll(validacionesStock);

        var erroresStock = errores
            .Where(x => x is not null)
            .Cast<Error>()
            .ToArray();

        if (erroresStock.Any())
        {
            return Result<Guid>.Failure(
                erroresStock);
        }

        var venta = new Venta
        {
            NumeroVenta = $"VEN-{Guid.NewGuid().ToString()[..6]}",
            FechaVenta = DateTime.UtcNow,
            Estado = "CREADA",
            Observacion = request.Observacion,
            TotalVenta = request.Detalles.Sum(
                x => x.Cantidad * x.PrecioUnitario),

            Detalles = request.Detalles
                .Select(x => new DetalleVenta
                {
                    ProductoId = x.ProductoId,
                    Cantidad = x.Cantidad,
                    PrecioUnitario = x.PrecioUnitario,
                    Subtotal = x.Cantidad * x.PrecioUnitario
                })
                .ToList()
        };

        _context.Ventas.Add(venta);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Venta {NumeroVenta} registrada", venta.NumeroVenta);

        await _publisher.PublishAsync( "venta.registrada",
            new VentaRegistradaEvent
            {
                NumeroVenta = venta.NumeroVenta,
                Items = venta.Detalles
                    .Select(x => new VentaRegistradaItemEvent
                    {
                        ProductoId = x.ProductoId,
                        Cantidad = x.Cantidad
                    })
                    .ToList()
            });

        _logger.LogInformation("Evento venta.registrada enviado");

        return Result<Guid>.Success(venta.Uid);
    }

    private async Task<int> ObtenerStockAsync(int productoId)
    {
        var client = _httpClientFactory.CreateClient("InventarioApi");

        var result = await client.GetFromJsonAsync<ApiResponse<int>>(
            $"api/inventario/stock/{productoId}");

        return result?.Data ?? 0;
    }
}
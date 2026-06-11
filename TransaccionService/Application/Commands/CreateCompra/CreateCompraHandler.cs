using BuildingBlocks.Application;
using BuildingBlocks.Messaging;
using MediatR;
using TransaccionService.Domain.Entities;
using TransaccionService.Domain.Errors;
using TransaccionService.Domain.Events;
using TransaccionService.Infrastructure.Persistence;

namespace TransaccionService.Application.Commands.CreateCompra;

public class CreateCompraHandler : IRequestHandler<CreateCompraCommand, Result<Guid>>
{
    private readonly TransaccionDbContext _context;
    private readonly IMessagePublisher _publisher;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CreateCompraHandler> _logger;

    public CreateCompraHandler(
        TransaccionDbContext context,
        IMessagePublisher publisher,
        ILogger<CreateCompraHandler> logger,
        IHttpClientFactory httpClientFactory        )
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Result<Guid>> Handle( CreateCompraCommand request,CancellationToken cancellationToken)
    {
        _logger.LogBusiness($"Registrando compra con {request.Detalles.Count} productos");

        var compra = new Compra
        {
            NumeroCompra =$"COM-{Guid.NewGuid().ToString()[..6]}",
            FechaCompra = DateTime.UtcNow,
            Estado = "CREADA",
            Observacion = request.Observacion,
            TotalCompra = request.Detalles.Sum(x => x.Cantidad * x.PrecioUnitario),

            Detalles = request.Detalles
                .Select(x => new DetalleCompra
                {
                    ProductoId = x.ProductoId,
                    Cantidad = x.Cantidad,
                    PrecioUnitario = x.PrecioUnitario,
                    Subtotal = x.Cantidad * x.PrecioUnitario
                })
                .ToList()
        };

        _context.Compras.Add(compra);

        await _context.SaveChangesAsync(
            cancellationToken);

        var productoClient =_httpClientFactory.CreateClient("ProductoApi");

        foreach (var item in compra.Detalles)
        {
            var response =
                await productoClient.PutAsJsonAsync(
                    "api/producto/actualizar-costo",
                    new
                    {
                        ProductoId = item.ProductoId,
                        Costo = item.PrecioUnitario
                    },
                    cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        _logger.LogInformation("Compra {NumeroCompra} registrada",compra.NumeroCompra);

        await _publisher.PublishAsync( "compra.registrada",
                                    new CompraRegistradaEvent
                                    {
                                        NumeroCompra = compra.NumeroCompra,
                                        Total = compra.TotalCompra,

                                        Items = compra.Detalles
                                            .Select(x => new CompraRegistradaItemEvent
                                            {
                                                ProductoId = x.ProductoId,
                                                Cantidad = x.Cantidad,
                                                PrecioUnitario = x.PrecioUnitario
                                            })
                                            .ToList()
                                    });
        _logger.LogInformation("Evento compra.registrada enviado");


        return Result<Guid>.Success(compra.Uid);
    }
}
using BuildingBlocks.Application.Logging;
using MediatR;
using ProductoService.Domain.Entities;
using ProductoService.Infrastructure.Persistence;

namespace ProductoService.Application.Commands.CreateProducto;

public class CreateProductoHandler : IRequestHandler<CreateProductoCommand, Guid>
{
    private readonly ProductoDbContext _context;
    private readonly ILogger<CreateProductoHandler> _logger;

    public CreateProductoHandler(
        ProductoDbContext context,
        ILogger<CreateProductoHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWithTrace(LogLevel.Information, "Creando producto");

        var producto = new Producto
        {
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Precio = request.Precio,
            Activo = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "system"
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogWithTrace(LogLevel.Information, $"Producto creado: {producto.Codigo}");

        return producto.Uid;
    }
}
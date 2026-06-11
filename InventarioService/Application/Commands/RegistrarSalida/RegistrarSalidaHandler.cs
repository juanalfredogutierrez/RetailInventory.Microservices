using MediatR;
using InventarioService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Application.Commands.RegistrarSalida;

public class RegistrarSalidaHandler : IRequestHandler<RegistrarSalidaCommand, bool>
{
    private readonly InventarioDbContext _context;
    private readonly ILogger<RegistrarSalidaHandler> _logger;
    public RegistrarSalidaHandler(InventarioDbContext context, ILogger<RegistrarSalidaHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(RegistrarSalidaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogBusiness($"Registrando salida para producto {request.ProductoId}");
        var stock = await _context.Existencias
                           .FirstOrDefaultAsync(x => x.ProductoId == request.ProductoId, cancellationToken);

        if (stock == null || stock.CantidadDisponible < request.Cantidad)
        {
            _logger.LogBusiness($"Stock insuficiente para producto {request.ProductoId}");
            throw new Exception("Stock insuficiente");
        }


        stock.CantidadDisponible -= request.Cantidad;
        stock.FechaActualizacion = DateTime.Now;
        _logger.LogBusiness($"Stock descontado para producto {request.ProductoId}");
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
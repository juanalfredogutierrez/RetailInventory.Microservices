using MediatR;
using InventarioService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Application.Commands.RegistrarSalida;

public class RegistrarSalidaHandler : IRequestHandler<RegistrarSalidaCommand, bool>
{
    private readonly InventarioDbContext _context;

    public RegistrarSalidaHandler(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RegistrarSalidaCommand request, CancellationToken cancellationToken)
    {
        var stock = await _context.Existencias
            .FirstOrDefaultAsync(x => x.ProductoId == request.ProductoId);

        if (stock == null || stock.CantidadDisponible < request.Cantidad)
            throw new Exception("Stock insuficiente");

        stock.CantidadDisponible -= request.Cantidad;
        stock.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
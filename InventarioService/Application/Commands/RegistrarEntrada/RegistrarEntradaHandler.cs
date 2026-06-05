using MediatR;
using InventarioService.Infrastructure.Persistence;
using InventarioService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Application.Commands.RegistrarEntrada;

public class RegistrarEntradaHandler : IRequestHandler<RegistrarEntradaCommand, bool>
{
    private readonly InventarioDbContext _context;

    public RegistrarEntradaHandler(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RegistrarEntradaCommand request, CancellationToken cancellationToken)
    {
        var stock = await _context.Existencias
            .FirstOrDefaultAsync(x => x.ProductoId == request.ProductoId);

        if (stock == null)
        {
            stock = new ExistenciaProducto
            {
                ProductoId = request.ProductoId,
                CantidadDisponible = request.Cantidad,
                FechaActualizacion = DateTime.UtcNow
            };

            _context.Existencias.Add(stock);
        }
        else
        {
            stock.CantidadDisponible += request.Cantidad;
            stock.FechaActualizacion = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
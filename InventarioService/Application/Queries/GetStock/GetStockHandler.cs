using MediatR;
using InventarioService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Application.Queries.GetStock;

public class GetStockHandler : IRequestHandler<GetStockQuery, int>
{
    private readonly InventarioDbContext _context;

    public GetStockHandler(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(GetStockQuery request, CancellationToken cancellationToken)
    {
        var stock = await _context.Existencias
            .FirstOrDefaultAsync(x => x.ProductoId == request.ProductoId);

        return stock?.CantidadDisponible ?? 0;
    }
}
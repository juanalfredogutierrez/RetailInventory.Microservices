using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductoService.Infrastructure.Persistence;

namespace ProductoService.Application.Queries.GetProductos;

public class GetProductosHandler : IRequestHandler<GetProductosQuery, List<object>>
{
    private readonly ProductoDbContext _context;

    public GetProductosHandler(ProductoDbContext context)
    {
        _context = context;
    }

    public async Task<List<object>> Handle(GetProductosQuery request, CancellationToken cancellationToken)
    {
        return await _context.Productos
            .Select(p => new
            {
                p.Uid,
                p.Codigo,
                p.Nombre,
                p.Precio,
                p.Activo
            })
            .ToListAsync(cancellationToken)
            .ContinueWith(t => t.Result.Cast<object>().ToList());
    }
}
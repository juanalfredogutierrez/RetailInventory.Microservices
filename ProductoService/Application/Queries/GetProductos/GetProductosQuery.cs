using MediatR;

namespace ProductoService.Application.Queries.GetProductos;

public record GetProductosQuery() : IRequest<List<object>>;
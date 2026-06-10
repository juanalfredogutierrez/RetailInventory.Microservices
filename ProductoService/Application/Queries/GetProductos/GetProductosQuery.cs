using BuildingBlocks.Application;
using MediatR;
using ProductoService.DTO;

namespace ProductoService.Application.Queries.GetProductos;

public record GetProductosQuery() : IRequest<Result<List<ProductoDto>>>;
using BuildingBlocks.Correlation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductoService.Application.Commands.CreateProducto;
using ProductoService.Application.Queries.GetProductos;


namespace ProductoService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductoCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(new { id, traceId = CorrelationContext.TraceId });
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetProductosQuery());
        return Ok(result);
    }
}
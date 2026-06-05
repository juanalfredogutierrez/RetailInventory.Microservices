using BuildingBlocks.Application.Logging;
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
    private readonly ILogger<ProductoController> _logger;

    public ProductoController(IMediator mediator, ILogger<ProductoController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductoCommand command)
    {
        _logger.LogWithTrace(LogLevel.Information, "Request CreateProducto");

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
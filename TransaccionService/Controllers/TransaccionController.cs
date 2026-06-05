using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransaccionService.Application.Commands.CreateCompra;

namespace TransaccionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransaccionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransaccionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("compra")]
    public async Task<IActionResult> CreateCompra(CreateCompraCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }
}
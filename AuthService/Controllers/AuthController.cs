using AuthService.Application.Commands.Login;
using BuildingBlocks.Application.Logging;
using BuildingBlocks.Correlation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        _logger.LogWithTrace(LogLevel.Information, "Login iniciado");

        var token = await _mediator.Send(command);

        _logger.LogWithTrace(LogLevel.Information, "Login exitoso");

        return Ok(new
        {
            token,
            traceId = CorrelationContext.TraceId
        });
    }
}
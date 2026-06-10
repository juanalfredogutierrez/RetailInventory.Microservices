using AuthService.Application.Commands.Login;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Security;
using BuildingBlocks.Application;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class LoginHandler: IRequestHandler<LoginCommand, Result<string>>
{
    private readonly AuthDbContext _context;
    private readonly JwtTokenGenerator _jwt;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        AuthDbContext context,
        JwtTokenGenerator jwt,
        ILogger<LoginHandler> logger)
    {
        _context = context;
        _jwt = jwt;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(LoginCommand request,CancellationToken cancellationToken)
    {
        var user = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(
                x => x.NombreUsuario == request.Username,
                cancellationToken);

        if (user is null)
        {
            _logger.LogBusiness(
                $"Login fallido para {request.Username}");

            return Result<string>.Failure(
                Errors.Unauthorized("Credenciales inválidas"));
        }

        var token = _jwt.GenerateToken(
            user.Id,
            user.NombreUsuario,
            user.Rol.Nombre);

        _logger.LogBusiness(
            $"Usuario {request.Username} autenticado");

        return token;
    }
}
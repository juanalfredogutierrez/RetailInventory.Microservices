using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, string>
{
    private readonly AuthDbContext _context;
    private readonly JwtTokenGenerator _jwt;

    public LoginHandler(AuthDbContext context, JwtTokenGenerator jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x =>
                x.NombreUsuario == request.Username,
                cancellationToken);

        if (user == null)
        {
            throw new Exception("Credenciales inválidas");
        }

        return _jwt.GenerateToken(
            user.Id,
            user.NombreUsuario,
            user.Rol.Nombre);
    }
}
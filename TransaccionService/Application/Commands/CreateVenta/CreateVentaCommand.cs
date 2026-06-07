using MediatR;
namespace TransaccionService.Application.Commands.CreateVenta;

public record CreateVentaCommand(List<DetalleVentaDto> Detalles, string Observacion) : IRequest<Guid>;
public record DetalleVentaDto(int ProductoId, int Cantidad, decimal PrecioUnitario);
namespace InventarioService.Domain.Entities;

public class EventoProcesado
{
    public Guid Id { get; set; }
    public Guid EventoId { get; set; }
    public string NombreEvento { get; set; }
    public DateTime FechaProcesamiento { get; set; }
}
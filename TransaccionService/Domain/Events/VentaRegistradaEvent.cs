using BuildingBlocks.Messaging;

namespace TransaccionService.Domain.Events;

public class VentaRegistradaEvent : IntegrationEvent
{
    public string NumeroVenta { get; set; }
    public decimal Total { get; set; }
}
using BuildingBlocks.Messaging;

namespace TransaccionService.Domain.Events;

public class CompraRegistradaEvent : IntegrationEvent
{
    public string NumeroCompra { get; set; }
    public decimal Total { get; set; }

}
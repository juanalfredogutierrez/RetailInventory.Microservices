namespace InventarioService.Domain.Events;

public class CompraRegistradaEvent
{
    public Guid EventId { get; set; }
    public string NumeroCompra { get; set; }
    public List<CompraItem> Items { get; set; }
}

public class CompraItem
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}
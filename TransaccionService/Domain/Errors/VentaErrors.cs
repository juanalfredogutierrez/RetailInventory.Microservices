using BuildingBlocks.Application;

namespace TransaccionService.Domain.Errors
{
    public static class VentaErrors
    {
        public static Error SinItems =>
            new(
                "Venta.SinItems",
                "La venta debe contener al menos un producto");

        public static Error StockInsuficiente(
            int productoId,
            int disponible,
            int solicitado)
            => new(
                "Venta.StockInsuficiente",
                $"Producto {productoId}. Disponible: {disponible}, Solicitado: {solicitado}");
    }
}

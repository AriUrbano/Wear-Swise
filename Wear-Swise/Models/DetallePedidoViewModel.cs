public class DetallePedidoViewModel
{
    public int IdDetallePedido { get; set; }
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; }
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    
    // Propiedad calculada para el subtotal
    public decimal Subtotal => Precio * Cantidad;
}
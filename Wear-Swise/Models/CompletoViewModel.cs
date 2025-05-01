namespace PrimerProyecto.Models
{
   public class PedidoCompletoViewModel
{
    public int IdPedido { get; set; }
    public DateTime FechaPedido { get; set; } // Usar esta propiedad para ambas fechas
    public decimal Total { get; set; }
    public string Estado { get; set; }
    public List<DetallePedidoViewModel> Items { get; set; }
    
    // Opcional: Propiedad calculada para mostrar como "FechaEntrega"
    public DateTime FechaEntrega => FechaPedido;
}
}
public class CarritoViewModel
{
    public List<DetallePedidoViewModel> Items { get; set; } = new List<DetallePedidoViewModel>();
    public List<PedidoCompletoViewModel> PedidosCompletados { get; set; } = new List<PedidoCompletoViewModel>();
    
    public decimal Total => Items.Sum(i => i.Subtotal);
}

public class PedidoCompletoViewModel
{
    public int IdPedido { get; set; }
    public DateTime FechaPedido { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; }
    public List<DetallePedidoViewModel> Items { get; set; }
}
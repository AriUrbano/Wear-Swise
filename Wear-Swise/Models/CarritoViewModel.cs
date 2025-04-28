using System.Collections.Generic;
using System.Linq;

namespace PrimerProyecto.Models
{
    public class CarritoViewModel
{
    public List<DetallePedidoViewModel> Items { get; set; } = new List<DetallePedidoViewModel>();
    public List<PedidoHistorialViewModel> Historial { get; set; } = new List<PedidoHistorialViewModel>();
    
    public decimal Total => Items.Sum(i => i.Subtotal);
}

public class PedidoHistorialViewModel
{
    public int IdPedido { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; }
    public List<DetallePedidoViewModel> Items { get; set; } = new List<DetallePedidoViewModel>();
}
}
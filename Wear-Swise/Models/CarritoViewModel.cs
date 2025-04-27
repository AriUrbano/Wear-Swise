using System.Collections.Generic;
using System.Linq;

namespace PrimerProyecto.Models
{
    public class CarritoViewModel
    {
        public List<DetallePedidoViewModel> Items { get; set; } = new List<DetallePedidoViewModel>();
        public decimal Total => Items.Sum(i => i.Subtotal);
    }
}
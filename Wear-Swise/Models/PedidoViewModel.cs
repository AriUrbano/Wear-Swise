// PedidoViewModel.cs
public class PedidoViewModel
{
    public int IdPedido { get; set; }
    public DateTime FechaPedido { get; set; }
    public string Estado { get; set; }
    public decimal Total { get; set; }
    public string Productos { get; set; }
}
namespace TuProyecto.Models
{
    public class Producto
    {
        public string NombreProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int idCategoria { get; set; }
    }
}

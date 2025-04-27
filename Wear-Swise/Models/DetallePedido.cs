using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimerProyecto.Models
{
    public class DetallePedido
    {
        [Key]
        public int id_detalle_pedido { get; set; }

        [ForeignKey("Pedido")]
        public int id_pedido { get; set; }

        [ForeignKey("Producto")]
        public int id_producto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal precio { get; set; }

        // Relaciones
        public Pedido Pedido { get; set; }
        public Producto Producto { get; set; }
    }
}
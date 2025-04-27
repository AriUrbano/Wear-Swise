using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimerProyecto.Models
{
    public class Pedido
    {
        [Key]
        public int id_pedido { get; set; }

        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }

        [Required]
        public DateTime fecha_pedido { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string estado { get; set; } = "Pendiente";

        [Column(TypeName = "decimal(10, 2)")]
        public decimal total { get; set; }

        // Relaciones
        public Usuario Usuario { get; set; }
        public ICollection<DetallePedido> DetallesPedido { get; set; }
    }
}
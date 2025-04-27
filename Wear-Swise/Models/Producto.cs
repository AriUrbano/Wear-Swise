using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimerProyecto.Models
{
    public class Producto
    {
        [Key]
        public int id_producto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string nombre_producto { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int stock { get; set; }

        [ForeignKey("Categoria")]
        public int? id_categoria { get; set; }

        [ForeignKey("Vendedor")]
        public int? id_vendedor { get; set; }

        public DateTime fecha_creacion { get; set; } = DateTime.Now;

        // Relaciones
        public Categoria Categoria { get; set; }
        public ICollection<DetallePedido> DetallesPedidos { get; set; }
    }
}
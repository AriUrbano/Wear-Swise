using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimerProyecto.Models
{
    public class Categoria
    {
        [Key]
        public int id_categoria { get; set; }

        [Required(ErrorMessage = "El nombre de categoría es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string nombre_categoria { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres")]
        public string descripcion { get; set; }

        // Relación con productos (una categoría puede tener muchos productos)
        public ICollection<Producto> Productos { get; set; }
    }
}
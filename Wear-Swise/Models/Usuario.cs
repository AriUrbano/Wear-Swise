using System.ComponentModel.DataAnnotations;

namespace PrimerProyecto.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string nombre_usuario { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        public string correo_electronico { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres")]
        public string contrasena { get; set; }

        // Constructor vacío
        public Usuario() { }

        // Constructor simplificado
        public Usuario(string nombre, string email, string password)
        {
            nombre_usuario = nombre;
            correo_electronico = email;
            contrasena = password;
        }
    }
}
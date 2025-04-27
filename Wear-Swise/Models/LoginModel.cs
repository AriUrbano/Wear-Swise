using System.ComponentModel.DataAnnotations;

namespace PrimerProyecto.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; }

        // AÑADE ESTA NUEVA PROPIEDAD
        public string ReturnUrl { get; set; }
    }
}
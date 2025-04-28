using System.ComponentModel.DataAnnotations;

namespace PrimerProyecto.Models
{
   public class RegistroModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
    [Display(Name = "Nombre completo")]
    public string NombreUsuario { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
    [Display(Name = "Correo electrónico")]
    public string CorreoElectronico { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmarContrasena { get; set; }
}
}
    

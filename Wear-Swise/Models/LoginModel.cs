using System.ComponentModel.DataAnnotations;

namespace PrimerProyecto.Models
{
    public class LoginModel
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email no válido")]
    public string CorreoElectronico { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; }

    public bool Recordarme { get; set; }
}
}
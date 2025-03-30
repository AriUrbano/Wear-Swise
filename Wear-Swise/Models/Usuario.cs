using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

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
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string contrasena { get; set; }

        public DateTime? fecha_nacimiento { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public int Teléfono  { get; set; } // Exactamente como en tu BD (con 1 'e')

        // Constructor vacío necesario
        public Usuario() { }

        // Constructor para registro
        public Usuario(string nombre, string email, string password, int telefono)
        {
            nombre_usuario = nombre;
            correo_electronico = email;
            contrasena = (password);
            Teléfono  = telefono;
        }
        public static string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password);
}

public static bool VerifyPassword(string inputPassword, string storedHash)
{
    return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
}


    }
}
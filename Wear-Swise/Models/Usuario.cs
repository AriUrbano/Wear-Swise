using System.ComponentModel.DataAnnotations;
using System.Text.Json;

public class Usuario
{
    public int   idUsuario { get; set; }
    public string NombreUsuario { get; set; }
    public string Email { get; set; }
    public string Contrasena { get; set; }
    public int Telefono { get; set; }
    public string ConfimarContrasena {get;set;}
    public Usuario(string nombreusuario, int idusuario, string email, string contrasena, int telefono)
    {
        idUsuario = idusuario;
        NombreUsuario = nombreusuario;
        Email= email;
        Contrasena = contrasena;
        Telefono = telefono;
    }
    public Usuario()
    {
    }
    public static Usuario? FromString(string? json)    {
        if (json is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Usuario>(json);
    }
}

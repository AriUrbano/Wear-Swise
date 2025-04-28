using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto.Models;

namespace PrimerProyecto.Controllers
{

public class AccountController : Controller
{
        private static string _connectionString = @"Server=.\;Database=Info360;Trusted_Connection=True;";


    [HttpGet]
public IActionResult Login(string returnUrl = null)
{
    // Si ya está autenticado, redirigir al Index
    if (User.Identity.IsAuthenticated)
    {
        return RedirectToAction("Index", "Home");
    }
    
    ViewData["ReturnUrl"] = returnUrl;
    return View();
}

   [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginModel model)
{
    if (User.Identity.IsAuthenticated)
    {
        return RedirectToAction("Index", "Home");
    }

    if (ModelState.IsValid)
    {
        var usuario = AutenticarUsuario(model.CorreoElectronico, model.Contrasena);
        
        if (usuario != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.id_usuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.nombre_usuario),
                new Claim(ClaimTypes.Email, usuario.correo_electronico)
            };

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false, // Cambiado a false para no recordar sesión
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Sesión de 30 minutos
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                authProperties);

            return RedirectToAction("Index", "Home");
        }
        
        ModelState.AddModelError(string.Empty, "Credenciales inválidas");
    }
    
    return View(model);
}
    private Usuario AutenticarUsuario(string email, string password)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var cmd = new SqlCommand(
                "SELECT id_usuario, nombre_usuario, correo_electronico " +
                "FROM usuarios WHERE correo_electronico = @Email AND contrasena = @Password", 
                cn);
            
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);
            
            cn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Usuario
                    {
                        id_usuario = reader.GetInt32(0),
                        nombre_usuario = reader.GetString(1),
                        correo_electronico = reader.GetString(2)
                    };
                }
            }
        }
        return null;
        
    }
    [HttpGet]
public IActionResult Registrar()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Registrar(RegistroModel model)
{
    if (ModelState.IsValid)
    {
        try
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                // Verificar si el email ya existe
                var cmdCheck = new SqlCommand(
                    "SELECT COUNT(*) FROM usuarios WHERE correo_electronico = @Email", cn);
                cmdCheck.Parameters.AddWithValue("@Email", model.CorreoElectronico);
                
                cn.Open();
                var exists = (int)cmdCheck.ExecuteScalar() > 0;
                
                if (exists)
                {
                    ModelState.AddModelError("CorreoElectronico", "Este correo electrónico ya está registrado");
                    return View(model);
                }

                // Registrar nuevo usuario
                var cmdInsert = new SqlCommand(
                    "INSERT INTO usuarios (nombre_usuario, correo_electronico, contrasena) " +
                    "VALUES (@Nombre, @Email, @Password); SELECT SCOPE_IDENTITY();", cn);
                
                cmdInsert.Parameters.AddWithValue("@Nombre", model.NombreUsuario);
                cmdInsert.Parameters.AddWithValue("@Email", model.CorreoElectronico);
                cmdInsert.Parameters.AddWithValue("@Password", model.Contrasena);
                
                var userId = cmdInsert.ExecuteScalar();
                
                TempData["RegistroExitoso"] = "Registro completado. Por favor inicie sesión.";
                return RedirectToAction("Login");
            }
        }
        catch (SqlException ex)
        {
            // Log del error (implementa esto según tu sistema de logging)
            ModelState.AddModelError("", "Ocurrió un error al registrar. Por favor intente nuevamente.");
            return View(model);
        }
    }
    return View(model);
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Logout()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToAction("Index", "Home");
}
}
}
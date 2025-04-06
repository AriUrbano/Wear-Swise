using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto.Models;

namespace PrimerProyecto.Controllers
{
    public class AccountController : Controller
    {
        // Cadena de conexión (ajusta según tu configuración)
        private static string _connectionString = @"Server=.\SQLEXPRESS;Database=Info360;Trusted_Connection=True;";        // GET: Muestra el formulario de login
        
       [HttpGet]
public IActionResult Login()
{
    // Si ya está autenticado, redirige al Home
    if (HttpContext.Session.GetInt32("user_id") != null)
    {
        return RedirectToAction("Index", "Home");
    }
    return View();
}

[HttpGet]
public IActionResult Registrar()
{
    // Si ya está autenticado, redirige al Home
    if (HttpContext.Session.GetInt32("user_id") != null)
    {
        return RedirectToAction("Index", "Home");
    }
    return View();
}

[HttpPost]
public IActionResult Login(string correo_electronico, string contrasena)
{
    try
    {
        using (SqlConnection cn = new SqlConnection(_connectionString))
        {
            // Consulta directa sin hash
            SqlCommand cmd = new SqlCommand(
                "SELECT id_usuario, nombre_usuario FROM usuarios " +
                "WHERE correo_electronico = @email AND contrasena = @pass", 
                cn);
                
            cmd.Parameters.AddWithValue("@email", correo_electronico);
            cmd.Parameters.AddWithValue("@pass", contrasena);  // <-- Contraseña en texto plano

            cn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    HttpContext.Session.SetInt32("user_id", reader.GetInt32(0));
                    HttpContext.Session.SetString("user_name", reader.GetString(1));
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        
        ViewData["Mensaje"] = "Credenciales incorrectas";
        return View();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"ERROR: {ex}");
        ViewData["Mensaje"] = "Error interno. Contacte al administrador.";
        return View();
    }
}

[HttpPost]
public IActionResult Registrar(Usuario usuario)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        using (SqlConnection cn = new SqlConnection(_connectionString))
        {
            SqlCommand cmd = new SqlCommand(
                "INSERT INTO usuarios (nombre_usuario, correo_electronico, contrasena) " +
                "VALUES (@nombre, @email, @pass)", cn);
            
            cmd.Parameters.AddWithValue("@nombre", usuario.nombre_usuario);
            cmd.Parameters.AddWithValue("@email", usuario.correo_electronico);
            cmd.Parameters.AddWithValue("@pass", usuario.contrasena);

            cn.Open();
            cmd.ExecuteNonQuery();
            
            TempData["RegistroExitoso"] = "¡Registro completado! Por favor inicia sesión";
            return RedirectToAction("Login");
        }
    }
    catch (SqlException ex) when (ex.Number == 2627)
    {
        ViewData["Mensaje"] = "El correo electrónico ya está registrado";
        return View(usuario);
    }
    catch (Exception ex)
    {
        ViewData["Mensaje"] = $"Error al registrar: {ex.Message}";
        return View(usuario);
    }
}
        // Cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        
    }
}
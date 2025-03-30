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
            return View();
        }

        [HttpPost]
public IActionResult Login(string correo_electronico, string contrasena)
{
    try
    {
        using (SqlConnection cn = new SqlConnection(_connectionString))
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT id_usuario, nombre_usuario, contrasena FROM usuarios WHERE correo_electronico = @email", 
                cn);
                
            cmd.Parameters.AddWithValue("@email", correo_electronico);
            cn.Open();

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string storedHash = reader["contrasena"].ToString();
                    if (Usuario.VerifyPassword(contrasena, storedHash)) // <-- Usa BCrypt
                    {
                        HttpContext.Session.SetInt32("user_id", reader.GetInt32(0));
                        HttpContext.Session.SetString("user_name", reader.GetString(1));
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
        }
        
        ViewData["Mensaje"] = "Credenciales incorrectas";
        return View();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"ERROR: {ex.ToString()}");
        ViewData["Mensaje"] = "Error interno. Contacte al administrador.";
        return View();
    }
}
        // GET: Muestra el formulario de registro
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        // POST: Procesa el registro
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
                    // Consulta que coincide con tu estructura de BD
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO usuarios (nombre_usuario, correo_electronico, contrasena, Telefeno) " +
                        "VALUES (@nombre, @email, @pass, @tel);", 
                        cn);
                    
                    // Parámetros exactamente como en tu BD
                    cmd.Parameters.AddWithValue("@nombre", usuario.nombre_usuario);
                    cmd.Parameters.AddWithValue("@email", usuario.correo_electronico);
                    cmd.Parameters.AddWithValue("@pass", Usuario.HashPassword(usuario.contrasena));
                    cmd.Parameters.AddWithValue("@tel", usuario.Teléfono );

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    
                    // Redirige al login con mensaje de éxito
                    TempData["RegistroExitoso"] = "¡Registro completado! Por favor inicia sesión";
                    return RedirectToAction("Login");
                }
            }
            catch (SqlException ex) when (ex.Number == 2627) // Violación de índice único
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
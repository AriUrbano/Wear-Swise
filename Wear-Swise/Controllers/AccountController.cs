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
        private readonly string _connectionString;

        public AccountController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: /Account/Login
       [HttpGet]
public IActionResult Login(string returnUrl = null)
{
    // Pasa el returnUrl al modelo
    var model = new LoginModel
    {
        ReturnUrl = returnUrl ?? Url.Content("~/")
    };
    return View(model);
}

        [HttpPost]
public async Task<IActionResult> Login(LoginModel model)
{
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

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            
            // Configuración CORRECTA del login:
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.Recordarme,
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                });

            // Redirección CORRECTA después del login:
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError("", "Credenciales inválidas");
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
    }
}
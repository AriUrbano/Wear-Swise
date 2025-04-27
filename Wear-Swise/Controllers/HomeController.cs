using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace PrimerProyecto.Controllers
{
    
    public class HomeController : Controller {

     private readonly string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
    
    
    public IActionResult Index()
    {
        return View();
    }
    
    [Authorize] // Ejemplo de acción que sí requiere autenticación
    public IActionResult MiPerfil()
    {
        return View();
    }
    }
}

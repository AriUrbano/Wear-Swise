using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace PrimerProyecto.Controllers
{
   public class HomeController : Controller
{
    [AllowAnonymous] // Permite acceso sin necesidad de login
    public IActionResult Index()
    {
        return View();
    }
}
}

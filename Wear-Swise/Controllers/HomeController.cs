using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto.Models ;

namespace PrimerProyecto.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

     public IActionResult Index()
    {
        // Verifica si hay sesión activa para mostrar contenido diferente
        if (HttpContext.Session.GetInt32("user_id") != null)
        {
            ViewBag.UserName = HttpContext.Session.GetString("user_name");
            ViewBag.IsLoggedIn = true;
        }
        else
        {
            ViewBag.IsLoggedIn = false;
        }
        return View();
    }

}


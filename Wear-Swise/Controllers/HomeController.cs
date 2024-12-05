using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto.Models;

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
        ViewBag.User = Usuario.FromString(HttpContext.Session.GetString("user"));
        if(ViewBag.User is null)
        {
            return RedirectToAction("Login", "Account");
        }
        return View();
    }

}


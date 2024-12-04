using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;

    public class AccountController : Controller
    {

        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }


        public ActionResult Registrar()
        {
            return View();
        }


        [HttpPost]
public ActionResult Registrar(Usuario oUsuario)
{

    if (oUsuario.Contrasena != oUsuario.ConfimarContrasena)
    {
        ViewData["Mensaje"] = "Las contraseñas no coinciden";
        return View();
    }

    var (registrado, mensaje) = BD.RegistrarUsuario(oUsuario);

    ViewData["Mensaje"] = mensaje;

    if (registrado)
    {
        return RedirectToAction("Login", "Account");
    }
    else
    {
        return View();
    }
}


        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            BD.ValidarUsuario(oUsuario);
            if (oUsuario.idUsuario != 0)
            {
                HttpContext.Session.SetString("user", (oUsuario.Email, oUsuario.Contrasena).ToString());
                return RedirectToAction("Index", "Home");
            }
            else {
                ViewData["Mensaje"] = "usuario no encontrado";
                return View();
            }

           
        }

    }

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
    public class AccountController : Controller
    {
        static string _connectionString = @"Server=localhost;DataBase=Wear-Swise;Trusted_Connection=true;";

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
            bool registrado;
            string mensaje;


            using (SqlConnection cn = new SqlConnection(_connectionString)) {

                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Email", oUsuario.Email);
                cmd.Parameters.AddWithValue("Contrasena", oUsuario.Contrasena);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();


            }    

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Account");
            }
            else {
                return View();
            }

        }

       [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            oUsuario.Contrasena = ConvertirSha256(oUsuario.Contrasena);

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {

                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Email);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Contrasena);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.idUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            }

            if (oUsuario.idUsuario != 0)
            {

                HttpContext.Session.SetString("user" , new Usuario(oUsuario.NombreUsuario, oUsuario.idUsuario, oUsuario.Email, oUsuario.Contrasena, oUsuario.Telefono, oUsuario.ConfimarContrasena).ToString());
                return RedirectToAction("Index", "Home");
            }
            else {
                ViewData["Mensaje"] = "usuario no encontrado";
                return View();
            } 

           
        }
        public IActionResult Logout()
    {
        HttpContext.Session.Remove("user");
        return RedirectToAction("Login");
    }
    public static string ConvertirSha256(string texto)
        {
            //using System.Text;
            //USAR LA REFERENCIA DE "System.Security.Cryptography"

            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }



    }

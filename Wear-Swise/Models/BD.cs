using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
public static class BD
{
    private static string _connectionString = @"Server=localhost;DataBase=Wear-Swise;Trusted_Connection=true;";

    public static void ValidarUsuario(Usuario oUsuario ) 
    {
        using (SqlConnection cn = new SqlConnection(_connectionString))
            {

                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Email);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Contrasena);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.idUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            }
    }
    public static (bool Registrado, string Mensaje) RegistrarUsuario(Usuario oUsuario)
{
    using (SqlConnection cn = new SqlConnection(_connectionString))
    {
        SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
        cmd.Parameters.AddWithValue("Correo", oUsuario.Email);
        cmd.Parameters.AddWithValue("Clave", oUsuario.Contrasena);
        cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        cmd.CommandType = CommandType.StoredProcedure;

        cn.Open();
        cmd.ExecuteNonQuery();

        bool registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
        string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

        return (registrado, mensaje);
    }
}


}
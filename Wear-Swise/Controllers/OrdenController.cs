using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PrimerProyecto.Models;

namespace PrimerProyecto.Controllers
{
    public class CarritoController : Controller  // Declaración de clase
    {
        private static string _connectionString = @"Server=.\;Database=Info360;Trusted_Connection=True;";
        private readonly ILogger<CarritoController> _logger;
    
    [HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Checkout()
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            // 1. Obtener el pedido pendiente
            var pedidoId = ObtenerPedidoPendiente(connection, userId);
            if (!pedidoId.HasValue)
            {
                TempData["Error"] = "No hay productos en el carrito";
                return RedirectToAction("Index");
            }

            // 2. Calcular el total del pedido
            var total = CalcularTotalPedido(connection, pedidoId.Value);

            // 3. Actualizar el pedido a "Completado"
            var updateCommand = new SqlCommand(
                "UPDATE pedidos SET estado = 'Completado', total = @total WHERE id_pedido = @pedidoId", 
                connection);
            
            updateCommand.Parameters.AddWithValue("@pedidoId", pedidoId.Value);
            updateCommand.Parameters.AddWithValue("@total", total);
            updateCommand.ExecuteNonQuery();

            TempData["Success"] = "Pedido completado exitosamente!";
            return RedirectToAction("Index");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al procesar el checkout");
        TempData["Error"] = "Ocurrió un error al procesar tu pedido";
        return RedirectToAction("Index");
    }
}

private decimal CalcularTotalPedido(SqlConnection connection, int pedidoId)
{
    var command = new SqlCommand(
        "SELECT SUM(precio * cantidad) FROM detalles_pedido WHERE id_pedido = @pedidoId", 
        connection);
    
    command.Parameters.AddWithValue("@pedidoId", pedidoId);
    return Convert.ToDecimal(command.ExecuteScalar());
}
}
}
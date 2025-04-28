using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace PrimerProyecto.Models;

public class ProductoController : Controller
{
    private static string _connectionString = @"Server=.\;Database=Info360;Trusted_Connection=True;";

    public IActionResult Listar()
    {
        List<Producto> productos = new List<Producto>();

        using (SqlConnection cn = new SqlConnection(_connectionString))
        {
            SqlCommand cmd = new SqlCommand("SELECT id_producto, nombre_producto, precio FROM productos", cn);
            cn.Open();
            
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    productos.Add(new Producto
                    {
                        id_producto = reader.GetInt32(0),
                        nombre_producto = reader.GetString(1),
                        precio = reader.GetDecimal(2)
                    });
                }
            }
        }

        return View(productos);
    }

    [HttpPost]
[Authorize]
public IActionResult AgregarAlCarrito(int idProducto, int cantidad = 1)
{
    try
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        using (var cn = new SqlConnection(_connectionString))
        {
            cn.Open();

            // 1. Verificar si el producto existe
            var cmdVerificar = new SqlCommand(
                "SELECT precio, nombre_producto FROM productos WHERE id_producto = @id", cn);
            cmdVerificar.Parameters.AddWithValue("@id", idProducto);
            
            using (var reader = cmdVerificar.ExecuteReader())
            {
                if (!reader.Read())
                {
                    TempData["Error"] = "Producto no encontrado";
                    return RedirectToAction("Listar");
                }
                var precio = reader.GetDecimal(0);
                var nombreProducto = reader.GetString(1);
            }

            // 2. Obtener o crear pedido (carrito)
            var cmdPedido = new SqlCommand(
                "SELECT id_pedido FROM pedidos WHERE id_usuario = @userId AND estado = 'Pendiente'", cn);
            cmdPedido.Parameters.AddWithValue("@userId", userId);
            
            var idPedido = cmdPedido.ExecuteScalar() as int?;
            
            if (!idPedido.HasValue)
            {
                cmdPedido.CommandText = @"
                    INSERT INTO pedidos (id_usuario, fecha_pedido, estado, total)
                    VALUES (@userId, GETDATE(), 'Pendiente', 0);
                    SELECT SCOPE_IDENTITY();";
                idPedido = Convert.ToInt32(cmdPedido.ExecuteScalar());
            }

            // 3. Agregar producto al carrito
            var cmdDetalle = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM detalles_pedido 
                          WHERE id_pedido = @idPedido AND id_producto = @idProducto)
                BEGIN
                    UPDATE detalles_pedido 
                    SET cantidad = cantidad + @cantidad
                    WHERE id_pedido = @idPedido AND id_producto = @idProducto
                END
                ELSE
                BEGIN
                    INSERT INTO detalles_pedido 
                    (id_pedido, id_producto, cantidad, precio)
                    VALUES (@idPedido, @idProducto, @cantidad, 
                           (SELECT precio FROM productos WHERE id_producto = @idProducto))
                END", cn);

            cmdDetalle.Parameters.AddWithValue("@idPedido", idPedido);
            cmdDetalle.Parameters.AddWithValue("@idProducto", idProducto);
            cmdDetalle.Parameters.AddWithValue("@cantidad", cantidad);
            cmdDetalle.ExecuteNonQuery();

            // 4. Actualizar total del pedido
            var cmdActualizar = new SqlCommand(
                "UPDATE pedidos SET total = (SELECT SUM(cantidad * precio) FROM detalles_pedido WHERE id_pedido = @idPedido) WHERE id_pedido = @idPedido", cn);
            cmdActualizar.Parameters.AddWithValue("@idPedido", idPedido);
            cmdActualizar.ExecuteNonQuery();

            TempData["Success"] = "Producto agregado al carrito";
        }

        return RedirectToAction("VerCarrito", "Carrito");
    }
    catch (Exception ex)
    {
        TempData["Error"] = $"Error al agregar al carrito: {ex.Message}";
        return RedirectToAction("Listar");
    }
}
}
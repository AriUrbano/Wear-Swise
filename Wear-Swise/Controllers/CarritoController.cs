using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Claims;
using PrimerProyecto.Models;

[Authorize]
public class CarritoController : Controller
{
    private static string _connectionString = @"Server=.\SQLEXPRESS;Database=Info360;Trusted_Connection=True;";
    private readonly ILogger<CarritoController> _logger;

    // GET: /Carrito
    public IActionResult Index()
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Debe iniciar sesión para ver el carrito";
            return RedirectToAction("Login", "Account");
        }

        var model = new CarritoViewModel { Items = new List<DetallePedidoViewModel>() };

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            
            // Verificar si hay un pedido pendiente primero
            var pedidoId = ObtenerPedidoPendiente(connection, userId);
            if (pedidoId.HasValue)
            {
                var query = @"SELECT dp.id_detalle_pedido, p.id_producto, p.nombre_producto, 
                            dp.precio, dp.cantidad, p.imagen_url
                            FROM detalles_pedido dp
                            JOIN productos p ON dp.id_producto = p.id_producto
                            JOIN pedidos pe ON dp.id_pedido = pe.id_pedido
                            WHERE pe.id_pedido = @pedidoId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@pedidoId", pedidoId.Value);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Items.Add(new DetallePedidoViewModel
                            {
                                IdDetallePedido = reader.GetInt32(0),
                                IdProducto = reader.GetInt32(1),
                                NombreProducto = reader.GetString(2),
                                Precio = reader.GetDecimal(3),
                                Cantidad = reader.GetInt32(4),
                            });
                        }
                    }
                }
            }
        }

        // Actualizar el contador del carrito
        ViewBag.CarritoCount = model.Items.Sum(i => i.Cantidad);
        
        return View(model);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al cargar el carrito");
        TempData["Error"] = "Error al cargar el carrito. Por favor, inténtelo nuevamente.";
        return View(new CarritoViewModel { Items = new List<DetallePedidoViewModel>() });
    }
}

    // POST: /Carrito/Agregar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Agregar(int idProducto, int cantidad = 1)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("Usuario no identificado");
            }

            if (cantidad <= 0)
            {
                throw new Exception("La cantidad debe ser mayor a cero");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Obtener o crear pedido pendiente
                var pedidoId = ObtenerPedidoPendiente(connection, userId) ?? CrearNuevoPedido(connection, userId);

                // Verificar si el producto ya está en el carrito
                var detalleExistente = ObtenerDetalleExistente(connection, pedidoId, idProducto);

                if (detalleExistente != null)
                {
                    // Actualizar cantidad si ya existe
                    ActualizarCantidad(connection, detalleExistente.IdDetallePedido, detalleExistente.Cantidad + cantidad);
                }
                else
                {
                    // Agregar nuevo producto al carrito
                    var precio = ObtenerPrecioProducto(connection, idProducto);
                    AgregarNuevoDetalle(connection, pedidoId, idProducto, cantidad, precio);
                }

                TempData["Success"] = "Producto agregado al carrito correctamente";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar producto al carrito");
            TempData["Error"] = $"Error al agregar producto: {ex.Message}";
            return RedirectToAction("Listar", "Productos");
        }
    }

    // POST: /Carrito/ActualizarCantidad
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ActualizarCantidad(int idDetalle, int cantidad)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("Usuario no identificado");
            }

            if (cantidad <= 0)
            {
                throw new Exception("La cantidad debe ser mayor a cero");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                ActualizarCantidad(connection, idDetalle, cantidad, userId);
                TempData["Success"] = "Cantidad actualizada correctamente";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cantidad");
            TempData["Error"] = $"Error al actualizar cantidad: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: /Carrito/Eliminar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int idDetalle)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("Usuario no identificado");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                var command = new SqlCommand(
                    "DELETE FROM detalles_pedido " +
                    "WHERE id_detalle_pedido = @idDetalle " +
                    "AND id_pedido IN (SELECT id_pedido FROM pedidos WHERE id_usuario = @userId AND estado = 'Pendiente')", 
                    connection);
                
                command.Parameters.AddWithValue("@idDetalle", idDetalle);
                command.Parameters.AddWithValue("@userId", userId);
                
                int affectedRows = command.ExecuteNonQuery();
                
                if (affectedRows == 0)
                {
                    throw new Exception("No se pudo eliminar el producto o no existe");
                }
                
                TempData["Success"] = "Producto eliminado del carrito correctamente";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto del carrito");
            TempData["Error"] = $"Error al eliminar producto: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    #region Métodos Auxiliares Privados

    private int? ObtenerPedidoPendiente(SqlConnection connection, string userId)
    {
        var command = new SqlCommand(
            "SELECT id_pedido FROM pedidos WHERE id_usuario = @userId AND estado = 'Pendiente'", 
            connection);
        
        command.Parameters.AddWithValue("@userId", userId);
        return command.ExecuteScalar() as int?;
    }

    private int CrearNuevoPedido(SqlConnection connection, string userId)
    {
        var command = new SqlCommand(
            "INSERT INTO pedidos (id_usuario, fecha_pedido, estado, total) " +
            "OUTPUT INSERTED.id_pedido " +
            "VALUES (@userId, GETDATE(), 'Pendiente', 0)", 
            connection);
        
        command.Parameters.AddWithValue("@userId", userId);
        return (int)command.ExecuteScalar();
    }

    private DetallePedidoViewModel ObtenerDetalleExistente(SqlConnection connection, int pedidoId, int productoId)
    {
        var command = new SqlCommand(
            "SELECT id_detalle_pedido, cantidad FROM detalles_pedido " +
            "WHERE id_pedido = @pedidoId AND id_producto = @productoId", 
            connection);
        
        command.Parameters.AddWithValue("@pedidoId", pedidoId);
        command.Parameters.AddWithValue("@productoId", productoId);
        
        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                return new DetallePedidoViewModel
                {
                    IdDetallePedido = reader.GetInt32(0),
                    Cantidad = reader.GetInt32(1)
                };
            }
        }
        return null;
    }

    private decimal ObtenerPrecioProducto(SqlConnection connection, int productoId)
    {
        var command = new SqlCommand(
            "SELECT precio FROM productos WHERE id_producto = @productoId", 
            connection);
        
        command.Parameters.AddWithValue("@productoId", productoId);
        return Convert.ToDecimal(command.ExecuteScalar());
    }

    private void AgregarNuevoDetalle(SqlConnection connection, int pedidoId, int productoId, int cantidad, decimal precio)
    {
        var command = new SqlCommand(
            "INSERT INTO detalles_pedido (id_pedido, id_producto, cantidad, precio) " +
            "VALUES (@pedidoId, @productoId, @cantidad, @precio)", 
            connection);
        
        command.Parameters.AddWithValue("@pedidoId", pedidoId);
        command.Parameters.AddWithValue("@productoId", productoId);
        command.Parameters.AddWithValue("@cantidad", cantidad);
        command.Parameters.AddWithValue("@precio", precio);
        command.ExecuteNonQuery();
    }

    private void ActualizarCantidad(SqlConnection connection, int detalleId, int cantidad, string userId = null)
    {
        var query = "UPDATE detalles_pedido SET cantidad = @cantidad WHERE id_detalle_pedido = @detalleId";
        
        if (userId != null)
        {
            query += " AND id_pedido IN (SELECT id_pedido FROM pedidos WHERE id_usuario = @userId AND estado = 'Pendiente')";
        }
        
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@cantidad", cantidad);
        command.Parameters.AddWithValue("@detalleId", detalleId);
        
        if (userId != null)
        {
            command.Parameters.AddWithValue("@userId", userId);
        }
        
        command.ExecuteNonQuery();
    }
    [HttpGet]
public IActionResult GetCartCount()
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { count = 0 });
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT SUM(cantidad) FROM detalles_pedido dp " +
                       "JOIN pedidos pe ON dp.id_pedido = pe.id_pedido " +
                       "WHERE pe.id_usuario = @userId AND pe.estado = 'Pendiente'";
            
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                var count = command.ExecuteScalar() as int? ?? 0;
                return Json(new { count });
            }
        }
    }
    catch
    {
        return Json(new { count = 0 });
    }
}
    #endregion
}
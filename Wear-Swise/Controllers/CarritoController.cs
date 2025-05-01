using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Claims;


[Authorize]
public class CarritoController : Controller
{
    private static string _connectionString = @"Server=.\SQLEXPRESS;Database=Info360;Trusted_Connection=True;";
    private readonly ILogger<CarritoController> _logger;

    public CarritoController(ILogger<CarritoController> logger)
    {
        _logger = logger;
    }

   public IActionResult Index()
{
    try
    {
        // 1. Verificar autenticación
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Debe iniciar sesión para ver el carrito";
            return RedirectToAction("Login", "Account");
        }

        var model = new CarritoViewModel();

        using (var connection = new SqlConnection(_connectionString))
        {
            try
            {
                connection.Open();
                _logger.LogInformation("Conexión a BD establecida correctamente");

                // 2. Obtener pedido pendiente con logging
                var pedidoId = ObtenerPedidoPendiente(connection, userId);
                _logger.LogInformation($"Pedido pendiente ID: {(pedidoId.HasValue ? pedidoId.Value.ToString() : "null")}");

                if (pedidoId.HasValue)
                {
                    // 3. Obtener items con manejo de errores específico
                    try
                    {
                        model.Items = ObtenerItemsCarrito(connection, pedidoId.Value);
                        _logger.LogInformation($"Se encontraron {model.Items.Count} items en el carrito");
                    }
                    catch (Exception itemsEx)
                    {
                        _logger.LogError(itemsEx, "Error al obtener items del carrito");
                        TempData["Error"] = "Error al cargar los productos del carrito";
                        model.Items = new List<DetallePedidoViewModel>();
                    }
                }

                // 4. Obtener historial con manejo de errores
                try
                {
                    model.PedidosCompletados = ObtenerHistorialPedidos(connection, userId);
                    _logger.LogInformation($"Se encontraron {model.PedidosCompletados.Count} pedidos completados");
                }
                catch (Exception histEx)
                {
                    _logger.LogError(histEx, "Error al obtener historial de pedidos");
                    TempData["Error"] = "Error al cargar el historial de pedidos";
                    model.PedidosCompletados = new List<PedidoCompletoViewModel>();
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error de SQL al cargar carrito");
                TempData["Error"] = "Error de conexión con la base de datos";
                return View(model);
            }
        }

        return View(model);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error inesperado al cargar carrito");
        TempData["Error"] = "Error inesperado al cargar el carrito";
        return View(new CarritoViewModel());
    }
}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Agregar(int idProducto, int cantidad = 1)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var pedidoId = ObtenerPedidoPendiente(connection, userId) ?? CrearNuevoPedido(connection, userId);
                var detalleExistente = ObtenerDetalleExistente(connection, pedidoId, idProducto);

                if (detalleExistente != null)
                {
                    ActualizarCantidad(connection, detalleExistente.IdDetallePedido, detalleExistente.Cantidad + cantidad);
                }
                else
                {
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ActualizarCantidad(int idDetalle, int cantidad)
    {
        try
        {
            if (cantidad <= 0)
            {
                TempData["Error"] = "La cantidad debe ser mayor a cero";
                return RedirectToAction(nameof(Index));
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                ActualizarCantidad(connection, idDetalle, cantidad);
                TempData["Success"] = "Cantidad actualizada correctamente";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cantidad");
            TempData["Error"] = "Error al actualizar la cantidad";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int idDetalle)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                EliminarDetalle(connection, idDetalle);
                TempData["Success"] = "Producto eliminado del carrito";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto del carrito");
            TempData["Error"] = "Error al eliminar el producto";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Usuario no identificado";
            return RedirectToAction("Index");
        }

         try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var pedidoId = ObtenerPedidoPendiente(connection, userId, transaction);
                    if (!pedidoId.HasValue)
                    {
                        TempData["Error"] = "No hay pedidos pendientes";
                        return RedirectToAction("Index");
                    }

                    // CALCULAR EL TOTAL ANTES DE USARLO
                    var total = CalcularTotalPedido(connection, pedidoId.Value, transaction);
                    
                    // Pasar el total al método CompletarPedido
                    CompletarPedido(connection, pedidoId.Value, total, transaction);

                    transaction.Commit();
                    TempData["Success"] = "¡Pedido completado con éxito!";
                }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Error en checkout");
                        TempData["Error"] = "Error al procesar el pedido";
                    }
                }
            }
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error general en checkout");
            TempData["Error"] = "Error al procesar tu solicitud";
            return RedirectToAction("Index");
        }
    }

    #region Métodos Auxiliares

    private int? ObtenerPedidoPendiente(SqlConnection connection, string userId, SqlTransaction transaction = null)
    {
        var command = new SqlCommand(
            "SELECT TOP 1 id_pedido FROM pedidos WHERE id_usuario = @userId AND estado = 'pendiente'", 
            connection, transaction);
        command.Parameters.AddWithValue("@userId", userId);
        return command.ExecuteScalar() as int?;
    }

    private int CrearNuevoPedido(SqlConnection connection, string userId, SqlTransaction transaction = null)
    {
        var command = new SqlCommand(
            "INSERT INTO pedidos (id_usuario, fecha_pedido, estado, total) OUTPUT INSERTED.id_pedido VALUES (@userId, GETDATE(), 'pendiente', 0)", 
            connection, transaction);
        command.Parameters.AddWithValue("@userId", userId);
        return (int)command.ExecuteScalar();
    }

    private List<DetallePedidoViewModel> ObtenerItemsCarrito(SqlConnection connection, int pedidoId, SqlTransaction transaction = null)
    {
        var command = new SqlCommand(
            @"SELECT 
                dp.id_detalle_pedido, 
                p.id_producto, 
                p.nombre_producto, 
                dp.precio, 
                dp.cantidad
              FROM detalles_pedido dp 
              JOIN productos p ON dp.id_producto = p.id_producto 
              WHERE dp.id_pedido = @pedidoId",
            connection, transaction);
        
        command.Parameters.AddWithValue("@pedidoId", pedidoId);

        var items = new List<DetallePedidoViewModel>();
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                items.Add(new DetallePedidoViewModel
                {
                    IdDetallePedido = (int)reader["id_detalle_pedido"],
                    IdProducto = (int)reader["id_producto"],
                    NombreProducto = reader["nombre_producto"].ToString(),
                    Precio = (decimal)reader["precio"],
                    Cantidad = (int)reader["cantidad"]
                });
            }
        }
        return items;
    }

    private List<PedidoCompletoViewModel> ObtenerHistorialPedidos(SqlConnection connection, string userId)
{
    var historial = new List<PedidoCompletoViewModel>();

    try
    {
        // 1. Primero obtenemos los pedidos completados
        var commandPedidos = new SqlCommand(
            @"SELECT p.id_pedido, p.fecha_pedido, p.total, p.estado
              FROM pedidos p
              WHERE p.id_usuario = @userId 
              AND p.estado IN ('entregado', 'enviado', 'cancelado')
              ORDER BY p.fecha_pedido DESC", 
            connection);
        
        commandPedidos.Parameters.AddWithValue("@userId", userId);

        // Lista temporal para almacenar IDs de pedidos
        var listaPedidos = new List<(int Id, DateTime Fecha, decimal Total, string Estado)>();

        using (var reader = commandPedidos.ExecuteReader())
        {
            while (reader.Read())
            {
                listaPedidos.Add((
                    (int)reader["id_pedido"],
                    (DateTime)reader["fecha_pedido"],
                    (decimal)reader["total"],
                    reader["estado"].ToString()
                ));
            }
        }

        // 2. Para cada pedido, obtener sus items
        foreach (var pedido in listaPedidos)
        {
            var items = new List<DetallePedidoViewModel>();
            
            try
            {
                var commandItems = new SqlCommand(
                    @"SELECT p.nombre_producto, dp.precio, dp.cantidad
                      FROM detalles_pedido dp
                      JOIN productos p ON dp.id_producto = p.id_producto
                      WHERE dp.id_pedido = @pedidoId", 
                    connection);
                
                commandItems.Parameters.AddWithValue("@pedidoId", pedido.Id);

                using (var itemReader = commandItems.ExecuteReader())
                {
                    while (itemReader.Read())
                    {
                        items.Add(new DetallePedidoViewModel
                        {
                            NombreProducto = itemReader["nombre_producto"].ToString(),
                            Precio = (decimal)itemReader["precio"],
                            Cantidad = (int)itemReader["cantidad"]
                        });
                    }
                }

                historial.Add(new PedidoCompletoViewModel
                {
                    IdPedido = pedido.Id,
                    FechaPedido = pedido.Fecha,
                    Total = pedido.Total,
                    Estado = pedido.Estado,
                    Items = items
                });
            }
            catch (Exception exItems)
            {
                _logger.LogError(exItems, $"Error al obtener items para pedido {pedido.Id}");
                // Si falla un pedido, continuamos con los demás
                continue;
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al obtener historial de pedidos");
        throw; // Relanzamos la excepción para manejarla en el método Index
    }

    return historial;
}

    private List<DetallePedidoViewModel> ObtenerItemsPedido(SqlConnection connection, int pedidoId)
    {
        var items = new List<DetallePedidoViewModel>();
        
        var command = new SqlCommand(
            @"SELECT p.nombre_producto, dp.precio, dp.cantidad 
              FROM detalles_pedido dp 
              JOIN productos p ON dp.id_producto = p.id_producto 
              WHERE dp.id_pedido = @pedidoId", 
            connection);
        
        command.Parameters.AddWithValue("@pedidoId", pedidoId);

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                items.Add(new DetallePedidoViewModel
                {
                    NombreProducto = reader["nombre_producto"].ToString(),
                    Precio = (decimal)reader["precio"],
                    Cantidad = (int)reader["cantidad"]
                });
            }
        }
        
        return items;
    }

    private DetallePedidoViewModel ObtenerDetalleExistente(SqlConnection connection, int pedidoId, int productoId, SqlTransaction transaction = null)
    {
        var command = new SqlCommand(
            "SELECT id_detalle_pedido, cantidad FROM detalles_pedido WHERE id_pedido = @pedidoId AND id_producto = @productoId", 
            connection, transaction);
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

    private decimal ObtenerPrecioProducto(SqlConnection connection, int productoId, SqlTransaction transaction = null)
    {
        var command = new SqlCommand("SELECT precio FROM productos WHERE id_producto = @productoId", connection, transaction);
        command.Parameters.AddWithValue("@productoId", productoId);
        return Convert.ToDecimal(command.ExecuteScalar());
    }

    private void AgregarNuevoDetalle(SqlConnection connection, int pedidoId, int productoId, int cantidad, decimal precio, SqlTransaction transaction = null)
    {
        var command = new SqlCommand(
            "INSERT INTO detalles_pedido (id_pedido, id_producto, cantidad, precio) VALUES (@pedidoId, @productoId, @cantidad, @precio)", 
            connection, transaction);
        command.Parameters.AddWithValue("@pedidoId", pedidoId);
        command.Parameters.AddWithValue("@productoId", productoId);
        command.Parameters.AddWithValue("@cantidad", cantidad);
        command.Parameters.AddWithValue("@precio", precio);
        command.ExecuteNonQuery();
    }

    private void ActualizarCantidad(SqlConnection connection, int detalleId, int cantidad, SqlTransaction transaction = null)
    {
        var command = new SqlCommand(
            "UPDATE detalles_pedido SET cantidad = @cantidad WHERE id_detalle_pedido = @detalleId", 
            connection, transaction);
        command.Parameters.AddWithValue("@cantidad", cantidad);
        command.Parameters.AddWithValue("@detalleId", detalleId);
        command.ExecuteNonQuery();
    }

    private void EliminarDetalle(SqlConnection connection, int detalleId, SqlTransaction transaction = null)
    {
        var command = new SqlCommand(
            "DELETE FROM detalles_pedido WHERE id_detalle_pedido = @detalleId", 
            connection, transaction);
        command.Parameters.AddWithValue("@detalleId", detalleId);
        command.ExecuteNonQuery();
    }

    private bool ProductoExisteYDisponible(SqlConnection connection, int productoId, int cantidad, SqlTransaction transaction = null)
    {
        var command = new SqlCommand(
            "SELECT COUNT(*) FROM productos WHERE id_producto = @productoId AND stock >= @cantidad", 
            connection, transaction);
        command.Parameters.AddWithValue("@productoId", productoId);
        command.Parameters.AddWithValue("@cantidad", cantidad);
        return (int)command.ExecuteScalar() > 0;
    }

   private void CompletarPedido(SqlConnection connection, int pedidoId, decimal total, SqlTransaction transaction = null)
{
    try
    {
        // 1. Actualizar stock - VERIFICAR QUE NO HAYA '/' EN LA CONSULTA
        var commandActualizarStock = new SqlCommand(
            @"UPDATE p
              SET p.stock = p.stock - dp.cantidad
              FROM productos p
              INNER JOIN detalles_pedido dp ON p.id_producto = dp.id_producto
              WHERE dp.id_pedido = @pedidoId", // Asegúrate que no hay caracteres especiales
            connection, transaction);
        
        commandActualizarStock.Parameters.AddWithValue("@pedidoId", pedidoId);
        commandActualizarStock.ExecuteNonQuery();

        // 2. Actualizar pedido - CONSULTA SIMPLIFICADA
        var commandCompletar = new SqlCommand(
            "UPDATE pedidos SET estado = 'entregado', total = @total WHERE id_pedido = @pedidoId",
            connection, transaction);
        
        commandCompletar.Parameters.AddWithValue("@pedidoId", pedidoId);
        commandCompletar.Parameters.AddWithValue("@total", total);
        commandCompletar.ExecuteNonQuery();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error al completar pedido {pedidoId}");
        throw;
    }
}
    private decimal CalcularTotalPedido(SqlConnection connection, int pedidoId, SqlTransaction transaction = null)
{
    var command = new SqlCommand(
        "SELECT SUM(precio * cantidad) FROM detalles_pedido WHERE id_pedido = @pedidoId", 
        connection, transaction);
    
    command.Parameters.AddWithValue("@pedidoId", pedidoId);
    
    var result = command.ExecuteScalar();
    return result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
}

    #endregion
}
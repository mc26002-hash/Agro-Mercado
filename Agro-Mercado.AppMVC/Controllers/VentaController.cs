using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class VentaController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public VentaController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        // ===========================
        // 🔹 INDEX
        // ===========================
        public async Task<IActionResult> Index(Venta? ventaSearch, int topRegistro = 5)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (ventaSearch == null)
                ventaSearch = new Venta();

            var query = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .AsQueryable();

            if (ventaSearch.ClienteId > 0)
                query = query.Where(v => v.ClienteId == ventaSearch.ClienteId);

            if (ventaSearch.EmpleadoId > 0)
                query = query.Where(v => v.EmpleadoId == ventaSearch.EmpleadoId);

            query = query.OrderByDescending(v => v.Id);

            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var ventas = await query.ToListAsync();

            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Empleados = _context.Empleados.ToList();

            return View(ventas);
        }

        // ===========================
        // 🔹 CREATE GET
        // ===========================
        public IActionResult Create()
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Productos = _context.Productos.ToList();
            ViewBag.Presentaciones = _context.ProductoPresentaciones.ToList();

            return View();
        }

        // ===========================
        // 🔹 CREATE POST
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Venta venta, List<DetalleVentum> detalles)
        {
            venta.Fecha = DateTime.Now;

            var empleadoSession = HttpContext.Session.GetInt32("EmpleadoId");

            if (empleadoSession == null)
                return Content("ERROR: No hay sesión");

            venta.EmpleadoId = empleadoSession.Value;

            // 🔥 FACTURA AUTOMÁTICA
            venta.FechaFactura = DateTime.Now;
            venta.NumeroFactura = $"FAC-{DateTime.Now:yyyyMMddHHmmss}";

            // 🔥 LIMPIAR VALIDACIONES
            ModelState.Remove("Fecha");
            ModelState.Remove("Empleado");
            ModelState.Remove("Cliente");
            ModelState.Remove("DetalleVenta");
            ModelState.Remove("NumeroFactura");
            ModelState.Remove("FechaFactura");

            foreach (var key in ModelState.Keys.ToList())
            {
                if (key.Contains("Venta") || key.Contains("Producto"))
                {
                    ModelState.Remove(key);
                }
            }

            // 🔥 VALIDAR CLIENTE
            if (venta.ClienteId == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un cliente");
            }

            // 🔥 VALIDAR DETALLES
            if (detalles == null || !detalles.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto");
            }

            foreach (var item in detalles ?? new List<DetalleVentum>())
            {
                if (item.ProductoId == 0)
                    ModelState.AddModelError("", "Debe seleccionar un producto");

                if (item.ProductoPresentacionId == 0)
                    ModelState.AddModelError("", "Debe seleccionar una presentación");

                if (item.Cantidad <= 0)
                    ModelState.AddModelError("", "Cantidad inválida");

                if (item.Precio <= 0)
                    ModelState.AddModelError("", "Precio inválido");
            }

            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return Content("ERRORES: " + string.Join(" | ", errores));
            }

            _context.Ventas.Add(venta);
            _context.SaveChanges();

            decimal subtotal = 0;

            foreach (var item in detalles ?? new List<DetalleVentum>())
            {
                item.SubTotal = item.Cantidad * item.Precio;
                subtotal += item.SubTotal;

                item.VentaId = venta.Id;
                _context.DetalleVenta.Add(item);

                var presentacion = _context.ProductoPresentaciones
                    .FirstOrDefault(p => p.Id == item.ProductoPresentacionId);

                var producto = _context.Productos
                    .FirstOrDefault(p => p.Id == item.ProductoId);

                if (presentacion != null && producto != null)
                {
                    decimal unidades = item.Cantidad * presentacion.Equivalencia;

                    if (producto.Stock < unidades)
                    {
                        return Content($"Stock insuficiente para {producto.Nombre}");
                    }

                    producto.Stock -= unidades;

                    var movimiento = new MovimientosInventario
                    {
                        ProductoId = item.ProductoId,
                        ProductoPresentacionId = item.ProductoPresentacionId,
                        TipoMovimiento = "Salida",
                        Cantidad = item.Cantidad,
                        Motivo = "Venta a cliente",
                        Fecha = DateTime.Now
                    };

                    _context.MovimientosInventarios.Add(movimiento);
                }
            }

            // 🔥 TOTALES
            venta.SubTotal = subtotal;
            venta.Iva = subtotal * 0.13m;
            venta.Total = venta.SubTotal + venta.Iva;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        // Get de edit
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var venta = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.ProductoPresentacion)
                .FirstOrDefault(v => v.Id == id);

            if (venta == null)
                return NotFound();

            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Productos = _context.Productos.ToList();
            ViewBag.Presentaciones = _context.ProductoPresentaciones.ToList();

            return View(venta);
        }

        // ============================
        // 🔴 Post de edit
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Venta venta, List<DetalleVentum>? detalles)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var ventaDb = _context.Ventas
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.ProductoPresentacion)
                .FirstOrDefault(v => v.Id == id);

            if (ventaDb == null)
                return NotFound();

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // ============================
                // 🔴 1. DEVOLVER STOCK VIEJO
                // ============================
                foreach (var item in ventaDb.DetalleVenta)
                {
                    var producto = _context.Productos
                        .FirstOrDefault(p => p.Id == item.ProductoId);

                    if (producto != null)
                    {
                        decimal unidades = item.Cantidad;

                        if (item.ProductoPresentacion != null)
                        {
                            unidades = item.Cantidad * item.ProductoPresentacion.Equivalencia;
                        }

                        // 🔥 DEVOLVER STOCK
                        producto.Stock += unidades;
                    }
                }

                // ============================
                // 🔴 2. ELIMINAR DETALLES VIEJOS
                // ============================
                _context.DetalleVenta.RemoveRange(ventaDb.DetalleVenta);

                // ============================
                // 🟢 3. AGREGAR NUEVOS DETALLES
                // ============================
                decimal subtotal = 0;

                foreach (var item in detalles ?? new List<DetalleVentum>())
                {
                    item.SubTotal = item.Cantidad * item.Precio;
                    subtotal += item.SubTotal;

                    item.VentaId = ventaDb.Id;
                    _context.DetalleVenta.Add(item);

                    var producto = _context.Productos
                        .FirstOrDefault(p => p.Id == item.ProductoId);

                    var presentacion = _context.ProductoPresentaciones
                        .FirstOrDefault(p => p.Id == item.ProductoPresentacionId);

                    if (producto != null && presentacion != null)
                    {
                        decimal unidades = item.Cantidad * presentacion.Equivalencia;

                        // 🔥 VALIDAR STOCK
                        if (producto.Stock < unidades)
                        {
                            throw new Exception($"Stock insuficiente para {producto.Nombre}");
                        }

                        // 🔥 DESCONTAR STOCK
                        producto.Stock -= unidades;

                        // 🔥 MOVIMIENTO
                        var movimiento = new MovimientosInventario
                        {
                            ProductoId = item.ProductoId,
                            ProductoPresentacionId = item.ProductoPresentacionId,
                            TipoMovimiento = "Salida",
                            Cantidad = item.Cantidad,
                            Motivo = "Edición de venta",
                            Fecha = DateTime.Now
                        };

                        _context.MovimientosInventarios.Add(movimiento);
                    }
                }

                // ============================
                // 🟢 4. ACTUALIZAR VENTA
                // ============================
                ventaDb.ClienteId = venta.ClienteId;
                ventaDb.MetodoPago = venta.MetodoPago;
                ventaDb.Fecha = DateTime.Now;

                ventaDb.SubTotal = subtotal;
                ventaDb.Iva = subtotal * 0.13m;
                ventaDb.Total = ventaDb.SubTotal + ventaDb.Iva;

                _context.SaveChanges();
                transaction.Commit();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Content("ERROR: " + ex.Message);
            }
        }

        public IActionResult Delete(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var venta = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.ProductoPresentacion)
                .FirstOrDefault(v => v.Id == id);

            if (venta == null)
                return NotFound();

            return View(venta);
        }


        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var venta = _context.Ventas
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.ProductoPresentacion)
                .FirstOrDefault(v => v.Id == id);

            if (venta == null)
                return NotFound();

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // ============================
                // 🔴 DEVOLVER STOCK
                // ============================
                foreach (var item in venta.DetalleVenta)
                {
                    var producto = _context.Productos
                        .FirstOrDefault(p => p.Id == item.ProductoId);

                    if (producto != null)
                    {
                        decimal unidades = item.Cantidad;

                        // 🔥 SI TIENE PRESENTACIÓN
                        if (item.ProductoPresentacion != null)
                        {
                            unidades = item.Cantidad * item.ProductoPresentacion.Equivalencia;
                        }

                        // 🔥 DEVOLVER STOCK
                        producto.Stock += unidades;
                    }
                }

                // ============================
                // 🔴 ELIMINAR DETALLES
                // ============================
                _context.DetalleVenta.RemoveRange(venta.DetalleVenta);

                // ============================
                // 🔴 ELIMINAR VENTA
                // ============================
                _context.Ventas.Remove(venta);

                _context.SaveChanges();
                transaction.Commit();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Content("ERROR: " + ex.Message);
            }
        }

        // ===========================
        // 🔹 DETAILS
        // ===========================
        public IActionResult Details(int id)
        {
            var venta = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.ProductoPresentacion)
                .FirstOrDefault(v => v.Id == id);

            if (venta == null)
                return NotFound();

            return View(venta);
        }
    }
}
using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class CompraController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public CompraController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        // ===========================
        // 🔹 INDEX
        // ===========================
        public async Task<IActionResult> Index(Compra? compraSearch, int topRegistro = 5)
        {
            if (!TieneAcceso(1, 6))
                return RedirectToAction("Index", "Home");

            if (compraSearch == null)
                compraSearch = new Compra();

            var query = _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Empleado)
                .AsQueryable();

            if (compraSearch.ProveedorId > 0)
                query = query.Where(c => c.ProveedorId == compraSearch.ProveedorId);

            if (compraSearch.EmpleadoId > 0)
                query = query.Where(c => c.EmpleadoId == compraSearch.EmpleadoId);

            query = query.OrderByDescending(c => c.Id);

            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var compras = await query.ToListAsync();

            ViewBag.Proveedores = _context.Proveedores.ToList();
            ViewBag.Empleados = _context.Empleados.ToList();

            return View(compras);
        }

        // ===========================
        // 🔹 MÉTODO AUXILIAR
        // ===========================
        private void CargarDatosViewBag()
        {
            ViewBag.Proveedores = _context.Proveedores.ToList();
            ViewBag.Productos = _context.Productos.ToList();
            ViewBag.Presentaciones = _context.ProductoPresentaciones.ToList();
        }

        // ===========================
        // 🔹 CREATE GET
        // ===========================
        public IActionResult Create()
        {
            if (!TieneAcceso(1, 6))
                return RedirectToAction("Index", "Home");
            CargarDatosViewBag();
            return View();
        }

        // ===========================
        // 🔹 CREATE POST
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Compra compra, List<DetalleCompra>? detalles)
        {
            if (!TieneAcceso(1, 6))
                return RedirectToAction("Index", "Home");
            try
            {
                compra.Fecha = DateTime.Now;

                var empleadoSession = HttpContext.Session.GetInt32("EmpleadoId");

                if (empleadoSession == null)
                {
                    ModelState.AddModelError("", "No hay sesión activa");
                }
                else
                {
                    compra.EmpleadoId = empleadoSession.Value;
                }

                // 🔥 LIMPIAR VALIDACIONES QUE MOLESTAN
                ModelState.Remove("Fecha");
                ModelState.Remove("Empleado");
                ModelState.Remove("Proveedor");
                ModelState.Remove("DetalleCompras");

                foreach (var key in ModelState.Keys.ToList())
                {
                    if (key.Contains("Compra") || key.Contains("Producto"))
                    {
                        ModelState.Remove(key);
                    }
                }

                // ===========================
                // 🔥 VALIDACIONES
                // ===========================

                // 🔴 PROVEEDOR (CLAVE)
                if (compra.ProveedorId == 0)
                {
                    ModelState.AddModelError("ProveedorId", "Debe seleccionar un proveedor");
                }

                // 🔴 DETALLES
                if (detalles == null || !detalles.Any())
                {
                    ModelState.AddModelError("", "Debe agregar al menos un producto");
                }

                // ===========================
                // 🔥 SI HAY ERRORES
                // ===========================
                if (!ModelState.IsValid)
                {
                    CargarDatosViewBag();
                    return View(compra);
                }

                // ===========================
                // 🔥 GUARDAR COMPRA
                // ===========================
                _context.Compras.Add(compra);
                _context.SaveChanges();

                decimal total = 0;

                foreach (var item in detalles ?? new List<DetalleCompra>())
                {
                    item.SubTotal = item.Cantidad * item.Precio;
                    total += item.SubTotal ?? 0;

                    item.CompraId = compra.Id;
                    _context.DetalleCompras.Add(item);

                    var presentacion = _context.ProductoPresentaciones
                        .FirstOrDefault(p => p.Id == item.ProductoPresentacionId);

                    var producto = _context.Productos
                        .FirstOrDefault(p => p.Id == item.ProductoId);

                    if (presentacion != null && producto != null)
                    {
                        decimal unidades = item.Cantidad * presentacion.Equivalencia;

                        producto.Stock += unidades;

                        var movimiento = new MovimientosInventario
                        {
                            ProductoId = item.ProductoId,
                            ProductoPresentacionId = item.ProductoPresentacionId,
                            TipoMovimiento = "Entrada",
                            Cantidad = item.Cantidad,
                            Motivo = "Compra a proveedor",
                            Fecha = DateTime.Now
                        };

                        _context.MovimientosInventarios.Add(movimiento);
                    }
                }

                compra.Total = total;

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);

                CargarDatosViewBag();
                return View(compra);
            }
        }

        // ===========================
        // 🔹 EDIT GET
        // ===========================
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1, 6))
                return RedirectToAction("Index", "Home");

            var compra = _context.Compras
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.Producto)
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.ProductoPresentacion)
                .FirstOrDefault(c => c.Id == id);

            if (compra == null)
                return NotFound();

            ViewBag.Proveedores = _context.Proveedores.ToList();
            ViewBag.Productos = _context.Productos.ToList();
            ViewBag.Presentaciones = _context.ProductoPresentaciones.ToList();

            return View(compra);
        }

        // ===========================
        // 🔹 EDIT POST
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Compra compra, List<DetalleCompra> detalles)
        {
            if (!TieneAcceso(1, 6))
                return RedirectToAction("Index", "Home");

            var compraDb = _context.Compras
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.ProductoPresentacion)
                .FirstOrDefault(c => c.Id == id);

            if (compraDb == null)
                return NotFound();

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // ============================
                // 🔴 1. RESTAR STOCK VIEJO
                // ============================
                foreach (var item in compraDb.DetalleCompras)
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

                        producto.Stock -= unidades;
                    }
                }

                // ============================
                // 🔴 2. ELIMINAR DETALLES VIEJOS
                // ============================
                _context.DetalleCompras.RemoveRange(compraDb.DetalleCompras);

                // ============================
                // 🟢 3. AGREGAR NUEVOS DETALLES
                // ============================
                decimal total = 0;

                foreach (var item in detalles)
                {
                    item.SubTotal = item.Cantidad * item.Precio;
                    total += item.SubTotal ?? 0;

                    item.CompraId = compraDb.Id;
                    _context.DetalleCompras.Add(item);

                    var producto = _context.Productos
                        .FirstOrDefault(p => p.Id == item.ProductoId);

                    var presentacion = _context.ProductoPresentaciones
                        .FirstOrDefault(p => p.Id == item.ProductoPresentacionId);

                    if (producto != null)
                    {
                        decimal unidades = item.Cantidad;

                        if (presentacion != null)
                        {
                            unidades = item.Cantidad * presentacion.Equivalencia;
                        }

                        // 🔥 SUMAR NUEVO STOCK
                        producto.Stock += unidades;
                    }
                }

                // ============================
                // 🟢 4. ACTUALIZAR COMPRA
                // ============================
                compraDb.ProveedorId = compra.ProveedorId;
                compraDb.Total = total;
                compraDb.Fecha = DateTime.Now;

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

        // ===========================
        // 🔹 DELETE GET
        // ===========================
        public IActionResult Delete(int id)
        {
            if (!TieneAcceso(1, 6))
                return RedirectToAction("Index", "Home");
            var compra = _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Empleado)
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.Producto)
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.ProductoPresentacion)
                .FirstOrDefault(c => c.Id == id);

            if (compra == null)
                return NotFound();

            return View(compra);
        }

        // ===========================
        // 🔹 DELETE POST
        // ===========================
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1, 6))
                return RedirectToAction("Index", "Home");

            var compra = _context.Compras
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.ProductoPresentacion)
                .FirstOrDefault(c => c.Id == id);

            if (compra == null)
                return NotFound();

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                foreach (var item in compra.DetalleCompras)
                {
                    var producto = _context.Productos
                        .FirstOrDefault(p => p.Id == item.ProductoId);

                    if (producto != null)
                    {
                        decimal unidades = item.Cantidad;

                        // 🔥 SI TIENE PRESENTACIÓN (CAJA, SACO, ETC)
                        if (item.ProductoPresentacion != null)
                        {
                            unidades = item.Cantidad * item.ProductoPresentacion.Equivalencia;
                        }

                        if (producto.Stock < unidades)
                        {
                            throw new Exception("Stock inconsistente, no se puede eliminar la compra.");
                        }

                        // 🔥 RESTAR DEL STOCK
                        producto.Stock -= unidades;

                    }
                }

                // 🔥 ELIMINAR DETALLES
                _context.DetalleCompras.RemoveRange(compra.DetalleCompras);

                // 🔥 ELIMINAR COMPRA
                _context.Compras.Remove(compra);

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
            if (!TieneAcceso(1, 6))
                return RedirectToAction("Index", "Home");

            var compra = _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Empleado)
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.Producto)
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.ProductoPresentacion) // 🔥 IMPORTANTE
                .FirstOrDefault(c => c.Id == id);

            if (compra == null)
                return NotFound();

            return View(compra);
        }
    }
}
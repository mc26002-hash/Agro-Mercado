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
        // 🔥 MÉTODO PARA FORMATEAR STOCK
        // ===========================
        private string FormatearStock(decimal stock)
        {
            int cajas = (int)(stock / 100);
            int unidades = (int)(stock % 100);

            if (cajas > 0 && unidades > 0)
                return $"{cajas} cajas + {unidades} unidades";

            if (cajas > 0)
                return $"{cajas} cajas";

            return $"{unidades} unidades";
        }

        // ===========================
        // 🔹 INDEX
        // ===========================
        public async Task<IActionResult> Index(Venta? ventaSearch, int topRegistro = 5)
        {
            if (!TieneAcceso(1, 8))
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

        public IActionResult Create()
        {
            if (!TieneAcceso(1, 8))
                return RedirectToAction("Index", "Home");

            var productos = _context.Productos.ToList();
            var presentaciones = _context.ProductoPresentaciones.ToList();

            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Productos = productos;
            ViewBag.Presentaciones = presentaciones;

            ViewBag.StockFormateado = productos
                .ToDictionary(
                    p => p.Id,
                    p =>
                    {
                        var presentacion = presentaciones
                            .FirstOrDefault(pp => pp.ProductoId == p.Id);

                        if (presentacion == null)
                            return $"{p.Stock} unidades";

                        decimal equivalencia = presentacion.Equivalencia;

                        decimal cantidad = Math.Floor(p.Stock / equivalencia);
                        decimal restante = p.Stock % equivalencia;

                        string nombre = presentacion.Nombre;

                        if (restante > 0)
                            return $"{cantidad} {nombre} + {restante} unidades";

                        return $"{cantidad} {nombre}";
                    }
                );

            return View();
        }

        // ===========================
        // 🔹 CREATE POST
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Venta venta, List<DetalleVentum>? detalles)
        {
            if (!TieneAcceso(1, 8))
                return RedirectToAction("Index", "Home");

            try
            {
                // 🔥 LIMPIAR NAVEGACIONES QUE ROMPEN VALIDACIÓN
                ModelState.Remove("Cliente");
                ModelState.Remove("Empleado");
                ModelState.Remove("DetalleVenta");

                venta.Fecha = DateTime.Now;

                var empleadoSession = HttpContext.Session.GetInt32("EmpleadoId");

                if (empleadoSession == null)
                {
                    ModelState.AddModelError("", "No hay sesión activa");
                }
                else
                {
                    venta.EmpleadoId = empleadoSession.Value;
                }

                venta.FechaFactura = DateTime.Now;
                venta.NumeroFactura = $"FAC-{DateTime.Now:yyyyMMddHHmmss}";

                // 🔥 VALIDACIONES MANUALES
                if (venta.ClienteId == 0)
                    ModelState.AddModelError("ClienteId", "Debe seleccionar un cliente");

                if (detalles == null || !detalles.Any())
                    ModelState.AddModelError("", "Debe agregar al menos un producto");

                // 🔥 VALIDAR STOCK
                foreach (var item in detalles ?? new List<DetalleVentum>())
                {
                    var producto = _context.Productos.FirstOrDefault(p => p.Id == item.ProductoId);
                    var presentacion = _context.ProductoPresentaciones.FirstOrDefault(p => p.Id == item.ProductoPresentacionId);

                    if (producto == null)
                    {
                        ModelState.AddModelError("", "Producto no encontrado");
                        continue;
                    }

                    if (presentacion == null)
                    {
                        ModelState.AddModelError("", $"Seleccione presentación para {producto.Nombre}");
                        continue;
                    }

                    decimal unidades = item.Cantidad * presentacion.Equivalencia;

                    if (producto.Stock < unidades)
                        ModelState.AddModelError("", $"Stock insuficiente para {producto.Nombre}");
                }

                // 🔥 SI HAY ERRORES → REGRESA A LA VISTA
                if (!ModelState.IsValid)
                {
                    // 🔥 RECARGAR DATOS (CLAVE)
                    var productos = _context.Productos.ToList();
                    var presentaciones = _context.ProductoPresentaciones.ToList();

                    ViewBag.Clientes = _context.Clientes.ToList();
                    ViewBag.Productos = productos;
                    ViewBag.Presentaciones = presentaciones;

                    ViewBag.StockFormateado = productos
                        .ToDictionary(
                            p => p.Id,
                            p =>
                            {
                                var presentacion = presentaciones
                                    .FirstOrDefault(pp => pp.ProductoId == p.Id);

                                if (presentacion == null)
                                    return $"{p.Stock} unidades";

                                decimal equivalencia = presentacion.Equivalencia;

                                decimal cantidad = Math.Floor(p.Stock / equivalencia);
                                decimal restante = p.Stock % equivalencia;

                                string nombre = presentacion.Nombre;

                                if (restante > 0)
                                    return $"{cantidad} {nombre} + {restante} unidades";

                                return $"{cantidad} {nombre}";
                            }
                        );

                    return View(venta);
                }

                // ===========================
                // 🔥 GUARDAR
                // ===========================
                _context.Ventas.Add(venta);
                _context.SaveChanges();

                decimal subtotal = 0;

                foreach (var item in detalles!)
                {
                    item.SubTotal = item.Cantidad * item.Precio;
                    subtotal += item.SubTotal;

                    item.VentaId = venta.Id;
                    _context.DetalleVenta.Add(item);

                    var producto = _context.Productos.First(p => p.Id == item.ProductoId);
                    var presentacion = _context.ProductoPresentaciones.First(p => p.Id == item.ProductoPresentacionId);

                    decimal unidades = item.Cantidad * presentacion.Equivalencia;

                    producto.Stock -= unidades;
                }

                venta.SubTotal = subtotal;
                venta.Iva = subtotal * 0.13m;
                venta.Total = venta.SubTotal + venta.Iva;

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);

                // 🔥 RECARGAR DATOS SI HAY ERROR
                var productos = _context.Productos.ToList();
                var presentaciones = _context.ProductoPresentaciones.ToList();

                ViewBag.Clientes = _context.Clientes.ToList();
                ViewBag.Productos = productos;
                ViewBag.Presentaciones = presentaciones;

                ViewBag.StockFormateado = productos.ToDictionary(p => p.Id, p => $"{p.Stock}");

                return View(venta);
            }
        }

        // ===========================
        // 🔹 EDIT GET
        // ===========================
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1, 8))
                return RedirectToAction("Index", "Home");

            var venta = _context.Ventas
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

            // 🔥 STOCK FORMATEADO
            ViewBag.StockFormateado = _context.Productos
                .ToDictionary(
                    p => p.Id,
                    p => FormatearStock(p.Stock)
                );

            return View(venta);
        }

        // ===========================
        // 🔹 EDIT POST (TU LÓGICA IGUAL)
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Venta venta, List<DetalleVentum>? detalles)
        {
            if (!TieneAcceso(1, 8))
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
                foreach (var item in ventaDb.DetalleVenta)
                {
                    var producto = _context.Productos.FirstOrDefault(p => p.Id == item.ProductoId);

                    if (producto != null)
                    {
                        decimal unidades = item.ProductoPresentacion != null
                            ? item.Cantidad * item.ProductoPresentacion.Equivalencia
                            : item.Cantidad;

                        producto.Stock += unidades;
                    }
                }

                _context.DetalleVenta.RemoveRange(ventaDb.DetalleVenta);

                decimal subtotal = 0;

                foreach (var item in detalles ?? new List<DetalleVentum>())
                {
                    item.SubTotal = item.Cantidad * item.Precio;
                    subtotal += item.SubTotal;

                    item.VentaId = ventaDb.Id;
                    _context.DetalleVenta.Add(item);

                    var producto = _context.Productos.FirstOrDefault(p => p.Id == item.ProductoId);
                    var presentacion = _context.ProductoPresentaciones.FirstOrDefault(p => p.Id == item.ProductoPresentacionId);

                    if (producto != null && presentacion != null)
                    {
                        decimal unidades = item.Cantidad * presentacion.Equivalencia;

                        if (producto.Stock < unidades)
                            throw new Exception($"Stock insuficiente para {producto.Nombre}");

                        producto.Stock -= unidades;
                    }
                }

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
        // ===========================
        // 🔹 DETAILS GET
        // ===========================
        public IActionResult Details(int id)
        {
            if (!TieneAcceso(1, 8))
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
        // ===========================
        // 🔹 DELETE GET
        // ===========================
        public IActionResult Delete(int id)
        {
            if (!TieneAcceso(1, 8))
                return RedirectToAction("Index", "Home");

            var venta = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefault(v => v.Id == id);

            if (venta == null)
                return NotFound();

            return View(venta);
        }
        // ===========================
        // 🔹 DELETE POST
        // ===========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1, 8))
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
                // 🔥 DEVOLVER STOCK
                foreach (var item in venta.DetalleVenta)
                {
                    var producto = _context.Productos.FirstOrDefault(p => p.Id == item.ProductoId);

                    if (producto != null)
                    {
                        decimal unidades = item.ProductoPresentacion != null
                            ? item.Cantidad * item.ProductoPresentacion.Equivalencia
                            : item.Cantidad;

                        producto.Stock += unidades;
                    }
                }

                // 🔥 ELIMINAR DETALLES
                _context.DetalleVenta.RemoveRange(venta.DetalleVenta);

                // 🔥 ELIMINAR VENTA
                _context.Ventas.Remove(venta);

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
    }


}
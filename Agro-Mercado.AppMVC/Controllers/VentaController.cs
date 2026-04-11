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

            // 🔥 STOCK FORMATEADO
            ViewBag.StockFormateado = _context.Productos
                .ToDictionary(
                    p => p.Id,
                    p => FormatearStock(p.Stock)
                );

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

            venta.FechaFactura = DateTime.Now;
            venta.NumeroFactura = $"FAC-{DateTime.Now:yyyyMMddHHmmss}";

            ModelState.Remove("Fecha");
            ModelState.Remove("Empleado");
            ModelState.Remove("Cliente");
            ModelState.Remove("DetalleVenta");
            ModelState.Remove("NumeroFactura");
            ModelState.Remove("FechaFactura");

            if (venta.ClienteId == 0)
                ModelState.AddModelError("", "Debe seleccionar un cliente");

            if (detalles == null || !detalles.Any())
                ModelState.AddModelError("", "Debe agregar productos");

            if (!ModelState.IsValid)
                return Content("Error en datos");

            _context.Ventas.Add(venta);
            _context.SaveChanges();

            decimal subtotal = 0;

            foreach (var item in detalles ?? new List<DetalleVentum>())
            {
                item.SubTotal = item.Cantidad * item.Precio;
                subtotal += item.SubTotal;

                item.VentaId = venta.Id;
                _context.DetalleVenta.Add(item);

                var producto = _context.Productos.FirstOrDefault(p => p.Id == item.ProductoId);
                var presentacion = _context.ProductoPresentaciones.FirstOrDefault(p => p.Id == item.ProductoPresentacionId);

                if (producto != null && presentacion != null)
                {
                    decimal unidades = item.Cantidad * presentacion.Equivalencia;

                    if (producto.Stock < unidades)
                        return Content($"Stock insuficiente para {producto.Nombre}");

                    producto.Stock -= unidades;
                }
            }

            venta.SubTotal = subtotal;
            venta.Iva = subtotal * 0.13m;
            venta.Total = venta.SubTotal + venta.Iva;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ===========================
        // 🔹 EDIT GET
        // ===========================
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1))
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
    }
}
using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class MovimientosInventarioController : Controller
    {
        private readonly AgroMercadoSprintContext _context;

        public MovimientosInventarioController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        public IActionResult Index(MovimientosInventario? movimientoSearch, int topRegistro = 5)
        {
            // 🔹 Evitar null
            if (movimientoSearch == null)
                movimientoSearch = new MovimientosInventario();

            // 🔹 Query base
            var query = _context.MovimientosInventarios
                .Include(m => m.Producto)
                .AsQueryable();

            // 🔍 FILTRO POR PRODUCTO
            if (movimientoSearch.ProductoId > 0)
                query = query.Where(m => m.ProductoId == movimientoSearch.ProductoId);

            // 🔍 FILTRO POR TIPO DE MOVIMIENTO
            if (!string.IsNullOrWhiteSpace(movimientoSearch.TipoMovimiento))
                query = query.Where(m => m.TipoMovimiento.Contains(movimientoSearch.TipoMovimiento));

            // 🔢 ORDENAR
            query = query.OrderByDescending(m => m.Fecha);

            // 🔥 CANTIDAD (0 = TODOS)
            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var movimientos = query.ToList();

            // 🔽 Para el select
            ViewBag.Productos = _context.Productos.ToList();

            // 🔥 Para mantener selección del combo
            ViewBag.TopRegistro = topRegistro;

            return View(movimientos);
        }

        // ============================
        // FORMULARIO ENTRADA INICIAL
        // ============================
        public IActionResult CrearEntradaInicial()
        {
            ViewBag.Productos = new SelectList(
                _context.Productos.Where(p => p.Activo == true),
                "Id",
                "Nombre"
            );

            ViewBag.Motivos = new SelectList(new List<string>
    {
        "Stock inicial"
    });

            return View();
        }

        // ============================
        // GUARDAR ENTRADA INICIAL
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearEntradaInicial(int productoId, int cantidad, string motivo)
        {
            if (cantidad <= 0)
                ModelState.AddModelError("", "La cantidad debe ser mayor a 0");

            if (string.IsNullOrWhiteSpace(motivo))
                ModelState.AddModelError("Motivo", "El motivo es obligatorio");

            var existe = _context.MovimientosInventarios
                .Any(m => m.ProductoId == productoId && m.TipoMovimiento == "Entrada Inicial");

            if (existe)
                ModelState.AddModelError("", "Este producto ya tiene stock inicial.");

            if (!ModelState.IsValid)
            {
                ViewBag.Productos = new SelectList(
                    _context.Productos.Where(p => p.Activo == true),
                    "Id",
                    "Nombre"
                );
                return View();
            }

            var producto = _context.Productos.Find(productoId);
            if (producto == null) return NotFound();

            producto.Stock = cantidad;

            var movimiento = new MovimientosInventario
            {
                ProductoId = productoId,
                TipoMovimiento = "Entrada Inicial",
                Cantidad = cantidad,
                Motivo = motivo, // 🔥 IMPORTANTE
                Fecha = DateTime.Now
            };

            _context.MovimientosInventarios.Add(movimiento);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // FORMULARIO ENTRADA NORMAL
        // ============================
        public IActionResult CrearEntrada()
        {
            ViewBag.Productos = new SelectList(
                _context.Productos.Where(p => p.Activo == true),
                "Id",
                "Nombre"
            );

            ViewBag.Motivos = new SelectList(new List<string>
    {
        "Compra a proveedor",
    "Ingreso manual",
    "Ajuste positivo de inventario"
    });

            return View();
        }

        // ============================
        // GUARDAR ENTRADA NORMAL
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearEntrada(int productoId, int cantidad, string motivo)
        {
            if (cantidad <= 0)
                ModelState.AddModelError("", "Cantidad inválida");

            if (string.IsNullOrWhiteSpace(motivo))
                ModelState.AddModelError("Motivo", "El motivo es obligatorio");

            if (!ModelState.IsValid)
            {
                ViewBag.Productos = new SelectList(
                    _context.Productos.Where(p => p.Activo == true),
                    "Id",
                    "Nombre"
                );
                return View();
            }

            var producto = _context.Productos.Find(productoId);
            if (producto == null) return NotFound();

            // SUMAR STOCK
            producto.Stock = (producto.Stock ?? 0) + cantidad;

            // CREAR MOVIMIENTO
            var movimiento = new MovimientosInventario
            {
                ProductoId = productoId,
                TipoMovimiento = "Entrada",
                Cantidad = cantidad,
                Motivo = motivo, // 🔥 IMPORTANTE
                Fecha = DateTime.Now
            };

            _context.MovimientosInventarios.Add(movimiento);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // FORMULARIO SALIDA
        // ============================
        public IActionResult CrearSalida()
        {
            ViewBag.Productos = new SelectList(
                _context.Productos.Where(p => p.Activo == true),
                "Id",
                "Nombre"
            );

            ViewBag.Motivos = new SelectList(new List<string>
    {
        "Venta",
    "Producto dañado",
    "Producto vencido",
    "Pérdida o robo",
    "Ajuste negativo de inventario"
    });

            return View();
        }

        // ============================
        // GUARDAR SALIDA
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearSalida(int productoId, int cantidad, string motivo)
        {
            if (cantidad <= 0)
                ModelState.AddModelError("", "Cantidad inválida");

            if (string.IsNullOrWhiteSpace(motivo))
                ModelState.AddModelError("Motivo", "El motivo es obligatorio");

            var producto = _context.Productos.Find(productoId);
            if (producto == null) return NotFound();

            if ((producto.Stock ?? 0) < cantidad)
                ModelState.AddModelError("", "No hay suficiente stock");

            if (!ModelState.IsValid)
            {
                ViewBag.Productos = new SelectList(
                    _context.Productos.Where(p => p.Activo == true),
                    "Id",
                    "Nombre"
                );
                return View();
            }

            producto.Stock -= cantidad;

            var movimiento = new MovimientosInventario
            {
                ProductoId = productoId,
                TipoMovimiento = "Salida",
                Cantidad = cantidad,
                Motivo = motivo, // 🔥 IMPORTANTE
                Fecha = DateTime.Now
            };

            _context.MovimientosInventarios.Add(movimiento);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // DETALLE
        // ============================
        public IActionResult Details(int id)
        {
            var movimiento = _context.MovimientosInventarios
                .Include(m => m.Producto)
                .FirstOrDefault(m => m.Id == id);

            if (movimiento == null) return NotFound();

            return View(movimiento);
        }

        // ============================
        // CONFIRMAR ELIMINAR
        // ============================
        public IActionResult Delete(int id)
        {
            var movimiento = _context.MovimientosInventarios
                .Include(m => m.Producto)
                .FirstOrDefault(m => m.Id == id);

            if (movimiento == null) return NotFound();

            return View(movimiento);
        }

        // ============================
        // ELIMINAR
        // ============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movimiento = _context.MovimientosInventarios.Find(id);

            if (movimiento != null)
            {
                var producto = _context.Productos.Find(movimiento.ProductoId);

                // 🔥 Revertir stock según tipo
                if (producto != null)
                {
                    if (movimiento.TipoMovimiento == "Entrada" || movimiento.TipoMovimiento == "Entrada Inicial")
                        producto.Stock -= movimiento.Cantidad;

                    if (movimiento.TipoMovimiento == "Salida")
                        producto.Stock += movimiento.Cantidad;
                }

                _context.MovimientosInventarios.Remove(movimiento);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
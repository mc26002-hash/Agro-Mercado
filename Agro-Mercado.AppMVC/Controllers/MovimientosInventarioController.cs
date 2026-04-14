using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class MovimientosInventarioController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public MovimientosInventarioController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        public IActionResult Index(MovimientosInventario? movimientoSearch, int topRegistro = 5)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            if (movimientoSearch == null)
                movimientoSearch = new MovimientosInventario();

            
            var query = _context.MovimientosInventarios
                .Include(m => m.Producto)
                .Include(m => m.ProductoPresentacion)
                .AsQueryable();

            
            if (movimientoSearch.ProductoId > 0)
                query = query.Where(m => m.ProductoId == movimientoSearch.ProductoId);

            
            if (!string.IsNullOrWhiteSpace(movimientoSearch.TipoMovimiento))
                query = query.Where(m => m.TipoMovimiento.Contains(movimientoSearch.TipoMovimiento));

            
            query = query.OrderByDescending(m => m.Fecha);

            
            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var movimientos = query.ToList();

            
            ViewBag.Productos = _context.Productos.ToList();

            
            ViewBag.TopRegistro = topRegistro;

            return View(movimientos);
        }

        
        public IActionResult CrearEntradaInicial()
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            ViewBag.Productos = new SelectList(
                _context.Productos.Where(p => p.Activo == true),
                "Id",
                "Nombre"
            );

            
            ViewBag.Presentaciones = _context.ProductoPresentaciones.ToList();

            ViewBag.Motivos = new SelectList(new List<string>
    {
        "Stock inicial"
    });

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearEntradaInicial(int productoId, int productoPresentacionId, decimal cantidad, string motivo)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            if (cantidad <= 0)
                ModelState.AddModelError("", "La cantidad debe ser mayor a 0");

            if (string.IsNullOrWhiteSpace(motivo))
                ModelState.AddModelError("Motivo", "El motivo es obligatorio");

            var producto = _context.Productos.Find(productoId);
            var presentacion = _context.ProductoPresentaciones.Find(productoPresentacionId);

            if (producto == null || presentacion == null)
                return NotFound();

            
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

                ViewBag.Presentaciones = _context.ProductoPresentaciones.ToList();

                ViewBag.Motivos = new SelectList(new List<string>
        {
            "Stock inicial"
        });

                return View();
            }

            
            decimal unidades = cantidad * presentacion.Equivalencia;

            
            producto.Stock = unidades;

            
            var movimiento = new MovimientosInventario
            {
                ProductoId = productoId,
                ProductoPresentacionId = productoPresentacionId,
                TipoMovimiento = "Entrada Inicial",
                Cantidad = cantidad,
                Motivo = motivo,
                Fecha = DateTime.Now
            };

            _context.MovimientosInventarios.Add(movimiento);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult CrearEntrada()
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            ViewBag.Productos = new SelectList(
                _context.Productos.Where(p => p.Activo == true),
                "Id",
                "Nombre"
            );

            ViewBag.Presentaciones = _context.ProductoPresentaciones.ToList();

            ViewBag.Motivos = new SelectList(new List<string>
    {
        "Ingreso manual",
        "Ajuste positivo de inventario"
    });

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearEntrada(int productoId, int productoPresentacionId, decimal cantidad, string motivo)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            if (cantidad <= 0)
                ModelState.AddModelError("", "Cantidad inválida");

            if (string.IsNullOrWhiteSpace(motivo))
                ModelState.AddModelError("Motivo", "El motivo es obligatorio");

            var producto = _context.Productos.Find(productoId);
            var presentacion = _context.ProductoPresentaciones.Find(productoPresentacionId);

            if (producto == null || presentacion == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Productos = new SelectList(
                    _context.Productos.Where(p => p.Activo == true),
                    "Id",
                    "Nombre"
                );

                ViewBag.Presentaciones = _context.ProductoPresentaciones.ToList();

                return View();
            }

            
            decimal unidades = cantidad * presentacion.Equivalencia;

            
            producto.Stock += unidades;

            
            var movimiento = new MovimientosInventario
            {
                ProductoId = productoId,
                ProductoPresentacionId = productoPresentacionId,
                TipoMovimiento = "Entrada",
                Cantidad = cantidad, 
                Motivo = motivo,
                Fecha = DateTime.Now
            };

            _context.MovimientosInventarios.Add(movimiento);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult CrearSalida()
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            ViewBag.Productos = new SelectList(
                _context.Productos.Where(p => p.Activo == true),
                "Id",
                "Nombre"
            );

            ViewBag.Presentaciones = new SelectList(
                _context.ProductoPresentaciones,
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

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearSalida(int productoId, int productoPresentacionId, decimal cantidad, string motivo)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            if (cantidad <= 0)
                ModelState.AddModelError("", "Cantidad inválida");

            if (string.IsNullOrWhiteSpace(motivo))
                ModelState.AddModelError("Motivo", "El motivo es obligatorio");

            var producto = _context.Productos.Find(productoId);
            if (producto == null) return NotFound();

            var presentacion = _context.ProductoPresentaciones.Find(productoPresentacionId);
            if (presentacion == null)
                ModelState.AddModelError("", "Debe seleccionar una presentación");

            decimal unidades = cantidad * (presentacion?.Equivalencia ?? 1);

            if (producto.Stock < unidades)
                ModelState.AddModelError("", "No hay suficiente stock");

            if (!ModelState.IsValid)
            {
                
                ViewBag.Productos = new SelectList(
                    _context.Productos.Where(p => p.Activo == true),
                    "Id",
                    "Nombre"
                );

                ViewBag.Presentaciones = new SelectList(
                    _context.ProductoPresentaciones,
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

            
            producto.Stock -= unidades;

            var movimiento = new MovimientosInventario
            {
                ProductoId = productoId,
                ProductoPresentacionId = productoPresentacionId,
                TipoMovimiento = "Salida",
                Cantidad = cantidad, 
                Motivo = motivo,
                Fecha = DateTime.Now
            };

            _context.MovimientosInventarios.Add(movimiento);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult Details(int id)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            var movimiento = _context.MovimientosInventarios
                .Include(m => m.Producto)
                .FirstOrDefault(m => m.Id == id);

            if (movimiento == null) return NotFound();

            return View(movimiento);
        }

        
        public IActionResult Delete(int id)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            var movimiento = _context.MovimientosInventarios
                .Include(m => m.Producto)
                .FirstOrDefault(m => m.Id == id);

            if (movimiento == null) return NotFound();

            return View(movimiento);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            var movimiento = _context.MovimientosInventarios.Find(id);

            if (movimiento != null)
            {
                var producto = _context.Productos.Find(movimiento.ProductoId);

                
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
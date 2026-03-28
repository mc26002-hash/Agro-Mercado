using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class ProductoController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public ProductoController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(Producto? productoSearch, int topRegistro = 10)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (productoSearch == null)
                productoSearch = new Producto();

            var query = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.UnidadMedida)
                .AsQueryable();

            // 🔍 Nombre
            if (!string.IsNullOrWhiteSpace(productoSearch.Nombre))
                query = query.Where(p => p.Nombre.Contains(productoSearch.Nombre));

            // 🔍 Categoría
            if (productoSearch.CategoriaId > 0)
                query = query.Where(p => p.CategoriaId == productoSearch.CategoriaId);

            // 🔍 Unidad de medida
            if (productoSearch.UnidadMedidaId > 0)
                query = query.Where(p => p.UnidadMedidaId == productoSearch.UnidadMedidaId);

            // 🔢 Orden + cantidad
            query = query
                .OrderByDescending(p => p.Id)
                .Take(topRegistro);

            var productos = await query.ToListAsync();

            // 🔽 Para los selects
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Unidades = _context.UnidadMedida.ToList();

            return View(productos);
        }

        // CREAR GET
        public IActionResult Create()
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Unidades = _context.UnidadMedida.ToList();

            return View();
        }

        // CREAR POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ModelState.Remove("Categoria");
            ModelState.Remove("UnidadMedida");

            if (ModelState.IsValid)
            {
                if (producto.FechaRegistro == null)
                    producto.FechaRegistro = DateTime.Now;

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Unidades = _context.UnidadMedida.ToList();

            return View(producto);
        }

        // EDITAR GET
        public async Task<IActionResult> Edit(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound();

            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Unidades = _context.UnidadMedida.ToList();

            return View(producto);
        }

        // EDITAR POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (id != producto.Id)
                return NotFound();

            ModelState.Remove("Categoria");
            ModelState.Remove("UnidadMedida");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Productos.Any(e => e.Id == producto.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Unidades = _context.UnidadMedida.ToList();

            return View(producto);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.UnidadMedida)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.UnidadMedida)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var producto = await _context.Productos.FindAsync(id);

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
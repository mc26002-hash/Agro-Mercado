using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class CategoriasController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public CategoriasController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(Categoria? categoriaSearch, int topRegistro = 5)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            if (categoriaSearch == null)
                categoriaSearch = new Categoria();

            var query = _context.Categorias.AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoriaSearch.Nombre))
                query = query.Where(c => c.Nombre.Contains(categoriaSearch.Nombre));

            if (!string.IsNullOrWhiteSpace(categoriaSearch.Descripcion))
                query = query.Where(c => c.Descripcion != null &&
                                         c.Descripcion.Contains(categoriaSearch.Descripcion));

            if (Request.Query["estado"] == "true")
                query = query.Where(c => c.Estado == true);

            if (Request.Query["estado"] == "false")
                query = query.Where(c => c.Estado == false);

            query = query.OrderByDescending(c => c.Id);

            
            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var lista = await query.ToListAsync();

            return View(lista);
        }

        
        public async Task<IActionResult> Details(int id)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        
        public IActionResult Create()
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(categoria);

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Categoría creada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            if (id != categoria.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(categoria);

            try
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categorias.Any(e => e.Id == categoria.Id))
                    return NotFound();
                else
                    throw;
            }

            TempData["Success"] = "Categoría actualizada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1, 6, 10))
                return RedirectToAction("Index", "Home");

            var categoria = await _context.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            
            if (categoria.Productos.Any())
            {
                TempData["Error"] = "No se puede eliminar porque tiene productos asociados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Categoría eliminada correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}
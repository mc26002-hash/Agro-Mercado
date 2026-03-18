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

        // LISTAR
        public async Task<IActionResult> Index()
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            var lista = await _context.Categorias.ToListAsync();
            return View(lista);
        }

        // DETALLE
        public async Task<IActionResult> Details(int id)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // CREAR GET
        public IActionResult Create()
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            return View();
        }

        // CREAR POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(categoria);

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Categoría creada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // EDITAR GET
        public async Task<IActionResult> Edit(int id)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // EDITAR POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (!TieneAcceso(1, 6, 8))
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

        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            var categoria = await _context.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            // Validar si tiene productos
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
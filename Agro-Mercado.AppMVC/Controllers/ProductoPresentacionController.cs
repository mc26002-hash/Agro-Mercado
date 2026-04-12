using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class ProductoPresentacionController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public ProductoPresentacionController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(ProductoPresentacion? search, int topRegistro = 10)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (search == null)
                search = new ProductoPresentacion();

            var query = _context.ProductoPresentaciones
                .Include(p => p.Producto)
                .AsQueryable();

            
            if (search.ProductoId > 0)
                query = query.Where(p => p.ProductoId == search.ProductoId);

            
            if (!string.IsNullOrWhiteSpace(search.Tipo))
                query = query.Where(p => p.Tipo.Contains(search.Tipo));

            
            if (!string.IsNullOrWhiteSpace(search.Nombre))
                query = query.Where(p => p.Nombre.Contains(search.Nombre));

            
            query = query.OrderByDescending(p => p.Id);

            
            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var lista = await query.ToListAsync();

            ViewBag.Productos = _context.Productos.ToList();

            return View(lista);
        }

        
        public IActionResult Create()
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ViewBag.Productos = _context.Productos.ToList();

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductoPresentacion model)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ModelState.Remove("Producto");

            if (ModelState.IsValid)
            {
                _context.ProductoPresentaciones.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Productos = _context.Productos.ToList();
            return View(model);
        }

        
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var data = _context.ProductoPresentaciones.Find(id);

            if (data == null)
                return NotFound();

            ViewBag.Productos = _context.Productos.ToList();

            return View(data);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductoPresentacion model)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ModelState.Remove("Producto");

            if (!ModelState.IsValid)
            {
                ViewBag.Productos = _context.Productos.ToList();
                return View(model);
            }

            var dbItem = _context.ProductoPresentaciones.Find(id);

            if (dbItem == null)
                return NotFound();

            dbItem.ProductoId = model.ProductoId;
            dbItem.Nombre = model.Nombre;
            dbItem.Equivalencia = model.Equivalencia;
            dbItem.Tipo = model.Tipo;
            dbItem.Activo = model.Activo;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        
        public IActionResult Delete(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var data = _context.ProductoPresentaciones
                .Include(p => p.Producto)
                .FirstOrDefault(p => p.Id == id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var data = _context.ProductoPresentaciones.Find(id);

            if (data != null)
            {
                _context.ProductoPresentaciones.Remove(data);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult Details(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var data = _context.ProductoPresentaciones
                .Include(p => p.Producto)
                .FirstOrDefault(p => p.Id == id);

            if (data == null)
                return NotFound();

            return View(data);
        }
    }
}
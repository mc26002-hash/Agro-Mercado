using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class ProveedoreController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public ProveedoreController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(Proveedore? proveedorSearch, int topRegistro = 5)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (proveedorSearch == null)
                proveedorSearch = new Proveedore();

            var query = _context.Proveedores
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(proveedorSearch.Nombre))
                query = query.Where(p => p.Nombre.Contains(proveedorSearch.Nombre));

            
            if (proveedorSearch.Activo)
                query = query.Where(p => p.Activo == proveedorSearch.Activo);

            
            query = query.OrderByDescending(p => p.Id);

            
            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var proveedores = await query.ToListAsync();

            return View(proveedores);
        }

        
        public IActionResult Create()
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Proveedore proveedor)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Proveedores.Add(proveedor);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(proveedor);
        }

        
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var proveedor = _context.Proveedores.Find(id);

            if (proveedor == null)
                return NotFound();

            return View(proveedor);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Proveedore proveedor)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(proveedor);

            var proveedorDb = _context.Proveedores.Find(id);

            if (proveedorDb == null)
                return NotFound();

            proveedorDb.Nombre = proveedor.Nombre;
            proveedorDb.Telefono = proveedor.Telefono;
            proveedorDb.Direccion = proveedor.Direccion;
            proveedorDb.Activo = proveedor.Activo;
            proveedorDb.Nit = proveedor.Nit;
            proveedorDb.Nrc = proveedor.Nrc;
            proveedorDb.CorreoElectronico = proveedor.CorreoElectronico;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        
        public IActionResult Delete(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var proveedor = _context.Proveedores.Find(id);

            if (proveedor == null)
                return NotFound();

            return View(proveedor);
        }

        
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var proveedor = _context.Proveedores.Find(id);

            if (proveedor != null)
            {
                _context.Proveedores.Remove(proveedor);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult Details(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var proveedor = _context.Proveedores
                .Include(p => p.Compras)
                .FirstOrDefault(p => p.Id == id);

            if (proveedor == null)
                return NotFound();

            return View(proveedor);
        }
    }
}
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

        // ===========================
        // 🔹 INDEX (FILTRO + TABLA)
        // ===========================
        public async Task<IActionResult> Index(Proveedore? proveedorSearch, int topRegistro = 5)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (proveedorSearch == null)
                proveedorSearch = new Proveedore();

            var query = _context.Proveedores
                .AsQueryable();

            // 🔍 Nombre
            if (!string.IsNullOrWhiteSpace(proveedorSearch.Nombre))
                query = query.Where(p => p.Nombre.Contains(proveedorSearch.Nombre));

            // 🔍 Activo
            if (proveedorSearch.Activo)
                query = query.Where(p => p.Activo == proveedorSearch.Activo);

            // 🔢 Orden
            query = query.OrderByDescending(p => p.Id);

            // 🔥 Top registros
            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var proveedores = await query.ToListAsync();

            return View(proveedores);
        }

        // ===========================
        // 🔹 CREATE GET
        // ===========================
        public IActionResult Create()
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            return View();
        }

        // ===========================
        // 🔹 CREATE POST
        // ===========================
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

        // ===========================
        // 🔹 EDIT GET
        // ===========================
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var proveedor = _context.Proveedores.Find(id);

            if (proveedor == null)
                return NotFound();

            return View(proveedor);
        }

        // ===========================
        // 🔹 EDIT POST
        // ===========================
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

        // ===========================
        // 🔹 DELETE GET
        // ===========================
        public IActionResult Delete(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var proveedor = _context.Proveedores.Find(id);

            if (proveedor == null)
                return NotFound();

            return View(proveedor);
        }

        // ===========================
        // 🔹 DELETE POST
        // ===========================
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

        // ===========================
        // 🔹 DETAILS
        // ===========================
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
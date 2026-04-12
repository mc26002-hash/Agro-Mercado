using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class RoleController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public RoleController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        
        public IActionResult Index(Role? roleSearch, int topRegistro = 5)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (roleSearch == null)
                roleSearch = new Role();

            var query = _context.Roles.AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(roleSearch.Nombre))
                query = query.Where(r => r.Nombre.Contains(roleSearch.Nombre));

            
            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var roles = query.ToList();

            ViewBag.TopRegistro = topRegistro;

            return View(roles);
        }

        
        public IActionResult Create()
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Role role)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Roles.Add(role);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(role);
        }

        
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var role = _context.Roles.Find(id);

            if (role == null)
                return NotFound();

            return View(role);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Role role)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(role);

            var roleDb = _context.Roles.Find(id);

            if (roleDb == null)
                return NotFound();

            
            roleDb.Nombre = role.Nombre;
            roleDb.Descripcion = role.Descripcion;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        
        public IActionResult Details(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var role = _context.Roles
                .Include(r => r.Empleados)
                .FirstOrDefault(r => r.Id == id);

            if (role == null)
                return NotFound();

            return View(role);
        }

        
        public IActionResult Delete(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var role = _context.Roles
                .Include(r => r.Empleados)
                .FirstOrDefault(r => r.Id == id);

            if (role == null)
                return NotFound();

            return View(role);
        }

        
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var role = _context.Roles
                .Include(r => r.Empleados)
                .FirstOrDefault(r => r.Id == id);

            if (role == null)
                return NotFound();

            
            if (role.Empleados.Any())
            {
                ModelState.AddModelError("", "No se puede eliminar el rol porque tiene empleados asignados.");
                return View("Delete", role);
            }

            _context.Roles.Remove(role);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
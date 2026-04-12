using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class EmpleadoController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public EmpleadoController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(Empleado? empleadoSearch, int topRegistro = 5)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (empleadoSearch == null)
                empleadoSearch = new Empleado();

            var query = _context.Empleados
                .Include(e => e.Rol)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(empleadoSearch.Nombre))
                query = query.Where(e => e.Nombre.Contains(empleadoSearch.Nombre));

            
            if (empleadoSearch.RolId > 0)
                query = query.Where(e => e.RolId == empleadoSearch.RolId);

            
            query = query.OrderByDescending(e => e.Id);

            
            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var empleados = await query.ToListAsync();

            ViewBag.Roles = _context.Roles.ToList();

            return View(empleados);
        }

        
        public IActionResult Create()
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ViewBag.Roles = _context.Roles.ToList();

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Empleado empleado)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ModelState.Remove("Rol");

            if (ModelState.IsValid)
            {
                var nuevoEmpleado = new Empleado
                {
                    Nombre = empleado.Nombre,
                    Correo = empleado.Correo,
                    Password = empleado.Password,
                    RolId = empleado.RolId,
                    Activo = empleado.Activo
                };

                _context.Empleados.Add(nuevoEmpleado);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Roles = _context.Roles.ToList();
            return View(empleado);
        }

        
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var empleado = _context.Empleados.Find(id);
            if (empleado == null)
            {
                return NotFound();
            }

            ViewBag.Roles = _context.Roles.ToList(); 

            return View(empleado);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Empleado empleado)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ModelState.Remove("Rol");

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _context.Roles.ToList(); 
                return View(empleado);
            }

            var empleadoDb = _context.Empleados.Find(id);

            if (empleadoDb == null)
                return NotFound();

            empleadoDb.Nombre = empleado.Nombre;
            empleadoDb.Correo = empleado.Correo;
            empleadoDb.Password = empleado.Password;
            empleadoDb.RolId = empleado.RolId;
            empleadoDb.Activo = empleado.Activo;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        
        public IActionResult Delete(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var empleado = _context.Empleados
                .Include(e => e.Rol)
                .FirstOrDefault(e => e.Id == id);

            if (empleado == null)
                return NotFound();

            return View(empleado);
        }

        
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var empleado = _context.Empleados.Find(id);

            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult Details(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var empleado = _context.Empleados
                .Include(e => e.Rol)
                .FirstOrDefault(e => e.Id == id);

            if (empleado == null)
                return NotFound();

            return View(empleado);
        }
    }
}
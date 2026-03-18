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

        // LISTAR
        public IActionResult Index()
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var empleados = _context.Empleados
                .Include(e => e.Rol)
                .ToList();

            return View(empleados);
        }

        // CREAR GET
        public IActionResult Create()
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ViewBag.Roles = _context.Roles.ToList();

            return View();
        }

        // CREAR POST
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

            ViewBag.Roles = _context.Roles.ToList(); // 👈 agregar esto
            return View(empleado);
        }

        // EDITAR GET
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var empleado = _context.Empleados.Find(id);
            if (empleado == null)
            {
                return NotFound();
            }

            ViewBag.Roles = _context.Roles.ToList(); // 👈 agregar esto

            return View(empleado);
        }

        // EDITAR POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Empleado empleado)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            ModelState.Remove("Rol");

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _context.Roles.ToList(); // 👈 importante
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

        // GET: Empleado/Delete/5
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

        // POST: Empleados/DeleteConfirmed/5
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

        // DETALLE
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
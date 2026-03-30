using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class UnidadesMedidaController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public UnidadesMedidaController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        public IActionResult Index(UnidadMedidum? unidadSearch, int topRegistro = 5)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            if (unidadSearch == null)
                unidadSearch = new UnidadMedidum();

            var query = _context.UnidadMedida.AsQueryable();

            // 🔍 Nombre
            if (!string.IsNullOrWhiteSpace(unidadSearch.Nombre))
                query = query.Where(u => u.Nombre.Contains(unidadSearch.Nombre));

            // 🔍 Abreviatura
            if (!string.IsNullOrWhiteSpace(unidadSearch.Abreviatura))
                query = query.Where(u => u.Abreviatura.Contains(unidadSearch.Abreviatura));

            // 🔍 Tipo
            if (!string.IsNullOrWhiteSpace(unidadSearch.Tipo))
                query = query.Where(u => u.Tipo == unidadSearch.Tipo);

            // 🔢 ORDEN
            query = query.OrderByDescending(u => u.Id);

            // 🔥 CANTIDAD (0 = TODOS)
            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var lista = query.ToList();

            // 🔥 IMPORTANTE (esto faltaba)
            ViewBag.Tipos = _context.UnidadMedida
                .Select(u => u.Tipo)
                .Distinct()
                .ToList();

            return View(lista);
        }

        // DETALLES
        public IActionResult Details(int id)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            var unidad = _context.UnidadMedida.Find(id);
            if (unidad == null)
                return NotFound();

            return View(unidad);
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
        public IActionResult Create(UnidadMedidum unidad)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(unidad);

            _context.UnidadMedida.Add(unidad);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // EDITAR GET
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            var unidad = _context.UnidadMedida.Find(id);
            if (unidad == null)
                return NotFound();

            return View(unidad);
        }

        // EDITAR POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, UnidadMedidum unidad)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(unidad);

            var unidadDb = _context.UnidadMedida.Find(id);
            if (unidadDb == null)
                return NotFound();

            unidadDb.Nombre = unidad.Nombre;
            unidadDb.Abreviatura = unidad.Abreviatura;
            unidadDb.Tipo = unidad.Tipo;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR GET
        public IActionResult Delete(int id)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            var unidad = _context.UnidadMedida.Find(id);
            if (unidad == null)
                return NotFound();

            return View(unidad);
        }

        // ELIMINAR POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1, 6, 8))
                return RedirectToAction("Index", "Home");

            var unidad = _context.UnidadMedida.Find(id);
            if (unidad != null)
            {
                _context.UnidadMedida.Remove(unidad);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
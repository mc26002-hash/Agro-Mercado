using Agro_Mercado.AppMVC.Controllers;
using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class DatosNegocioController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public DatosNegocioController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var datos = _context.DatosNegocios.FirstOrDefault();

            if (datos == null)
            {
                datos = new DatosNegocio();
                _context.DatosNegocios.Add(datos);
                _context.SaveChanges();
            }

            return View(datos);
        }

        
        public IActionResult Edit(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var datos = _context.DatosNegocios.Find(id);
            if (datos == null)
                return NotFound();

            return View(datos);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DatosNegocio datos)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(datos);

            var datosDb = _context.DatosNegocios.Find(id);
            if (datosDb == null)
                return NotFound();

            datosDb.Nombre = datos.Nombre;
            datosDb.Direccion = datos.Direccion;
            datosDb.Telefono = datos.Telefono;
            datosDb.Correo = datos.Correo;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
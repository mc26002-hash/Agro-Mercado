using Agro_Mercado.AppMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class ClienteController : BaseController
    {
        private readonly AgroMercadoSprintContext _context;

        public ClienteController(AgroMercadoSprintContext context)
        {
            _context = context;
        }

        // ===========================
        // 🔹 INDEX (FILTRO)
        // ===========================
        public async Task<IActionResult> Index(Cliente clienteSearch, int topRegistro = 10)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (clienteSearch == null)
                clienteSearch = new Cliente();

            var query = _context.Clientes.AsQueryable();

            // 🔍 FILTROS
            if (!string.IsNullOrWhiteSpace(clienteSearch.Nombre))
                query = query.Where(c => c.Nombre.Contains(clienteSearch.Nombre));

            if (!string.IsNullOrWhiteSpace(clienteSearch.Dui))
                query = query.Where(c => c.Dui != null && c.Dui.Contains(clienteSearch.Dui));

            if (clienteSearch.Activo)
                query = query.Where(c => c.Activo == true);

            query = query.OrderByDescending(c => c.Id);

            if (topRegistro > 0)
                query = query.Take(topRegistro);

            var clientes = await query.ToListAsync();

            return View(clientes);
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
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return View(cliente);
            }

            cliente.Activo = true;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // 🔹 EDIT GET
        // ===========================
        public async Task<IActionResult> Edit(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // ===========================
        // 🔹 EDIT POST
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            if (id != cliente.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return Content("ERRORES: " + string.Join(" | ", errores));
            }

            try
            {
                var clienteDb = await _context.Clientes.FindAsync(id);

                if (clienteDb == null)
                    return NotFound();

                clienteDb.Nombre = cliente.Nombre;
                clienteDb.Telefono = cliente.Telefono;
                clienteDb.Direccion = cliente.Direccion;
                clienteDb.Dui = cliente.Dui;
                clienteDb.Activo = cliente.Activo;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // 🔹 DELETE GET
        // ===========================
        public async Task<IActionResult> Delete(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // ===========================
        // 🔹 DELETE POST
        // ===========================
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            try
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return Content("No se puede eliminar, este cliente tiene ventas asociadas.");
            }

            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // 🔹 DETAILS
        // ===========================
        public async Task<IActionResult> Details(int id)
        {
            if (!TieneAcceso(1))
                return RedirectToAction("Index", "Home");

            var cliente = await _context.Clientes
                .Include(c => c.Venta)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }
    }
}
using Agro_Mercado.AppMVC.Models;  // verifica que coincida con tu namespace
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class AccountController : Controller
{
    private readonly AgroMercadoSprintContext _context;

    public AccountController(AgroMercadoSprintContext context)
    {
        _context = context;
    }

    // ===============================
    // 1. Mostrar vista Login (GET)
    // ===============================
    public IActionResult Login()
    {
        return View();
    }

    // ===============================
    // 2. Procesar Login (POST)
    // ===============================
    [HttpPost]
    public IActionResult Login(string correo, string password)
    {
        var usuario = _context.Empleados
            .FirstOrDefault(u => u.Correo == correo && u.Password == password);

        if (usuario != null)
        {
            HttpContext.Session.SetString("Usuario", usuario.Nombre);

            // 🔥 Validar nullable
            if (usuario.RolId.HasValue)
            {
                HttpContext.Session.SetInt32("RolId", usuario.RolId.Value);
            }

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Correo o contraseña incorrectos";
        return View();
    }

    // ===============================
    // 3. Cerrar sesión
    // ===============================

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}
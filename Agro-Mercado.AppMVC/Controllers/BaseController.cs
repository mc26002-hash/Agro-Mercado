using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Agro_Mercado.AppMVC.Controllers
{
    public class BaseController : Controller
    {
        protected bool TieneAcceso(params int[] rolesPermitidos)
        {
            var rolId = HttpContext.Session.GetInt32("RolId");

            if (rolId == null)
                return false;

            return rolesPermitidos.Contains(rolId.Value);
        }


        protected bool EsAdmin()
        {
            var rol = HttpContext.Session.GetInt32("RolId");
            return rol == 1; 
        }

    }
}
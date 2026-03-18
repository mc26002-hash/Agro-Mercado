using System.ComponentModel.DataAnnotations;

namespace Agro_Mercado.AppMVC.Models
{
    public class LoginViewModel
    {
        [Required]
        public string? Correo { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
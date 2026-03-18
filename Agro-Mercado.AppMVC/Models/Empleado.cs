using Agro_Mercado.AppMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agro_Mercado.AppMVC.Models;

public partial class Empleado
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del empleado es obligatorio")]
    [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres")]
    public string Nombre { get; set; } = null!;

    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [StringLength(100)]
    public string? Correo { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Debe asignar un rol al empleado")]
    public int RolId { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual Role Rol { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
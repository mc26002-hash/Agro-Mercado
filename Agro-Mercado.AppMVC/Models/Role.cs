using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agro_Mercado.AppMVC.Models;

public partial class Role
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del rol es obligatorio")]
    [StringLength(20, ErrorMessage = "El nombre no puede superar los 20 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}

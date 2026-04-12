using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agro_Mercado.AppMVC.Models;

public partial class Cliente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres")]
    public string Nombre { get; set; } = null!;

    [Phone(ErrorMessage = "El número de teléfono no es válido")]
    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(100, ErrorMessage = "La dirección es demasiado larga")]
    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true; 

    
    [StringLength(10, ErrorMessage = "El DUI no puede superar los 10 caracteres")]
    public string? Dui { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
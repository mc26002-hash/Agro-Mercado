using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agro_Mercado.AppMVC.Models;

public partial class Proveedore
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del proveedor es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El telefono es obligatorio")]
    [Phone(ErrorMessage = "El número de teléfono no es válido")]
    [StringLength(20)]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [StringLength(100, ErrorMessage = "La dirección es demasiado larga")]
    public string Direccion { get; set; } = null!;

    public bool Activo { get; set; }

    [Required(ErrorMessage = "El NIT es obligatorio")]
    [StringLength(20, ErrorMessage = "El NIT no puede superar los 17 caracteres")]
    public string? Nit { get; set; }

    [Required(ErrorMessage = "El NRC es obligatorio")]
    [StringLength(20)]
    public string? Nrc { get; set; }

    [Required(ErrorMessage = "El correo electronico es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [StringLength(100)]
    public string? CorreoElectronico { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
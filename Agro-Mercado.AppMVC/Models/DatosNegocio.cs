using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agro_Mercado.AppMVC.Models;

public partial class DatosNegocio
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del negocio es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres")]
    public string? Direccion { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [Phone(ErrorMessage = "El número de teléfono no es válido")]
    [StringLength(15, ErrorMessage = "El teléfono no puede superar los 15 caracteres")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres")]
    public string? Correo { get; set; }
}
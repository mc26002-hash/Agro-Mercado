using Agro_Mercado.AppMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agromercado.AppMVC.Models;

public partial class UnidadMedidum
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la unidad de medida es obligatorio")]
    [StringLength(25, ErrorMessage = "El nombre no puede superar los 25 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La abreviatura es obligatoria")]
    [StringLength(50, ErrorMessage = "La abreviatura no puede superar los 50 caracteres")]
    public string Abreviatura { get; set; } = null!;

    [Required(ErrorMessage = "Debe especificar el tipo de unidad")]
    [StringLength(20, ErrorMessage = "El tipo no puede superar los 20 caracteres")]
    public string Tipo { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
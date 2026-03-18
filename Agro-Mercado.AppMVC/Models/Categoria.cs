using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agro_Mercado.AppMVC.Models;

public partial class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
    [StringLength(25, ErrorMessage = "El nombre no puede superar los 25 caracteres")]
    public string Nombre { get; set; } = null!;

    [StringLength(50, ErrorMessage = "La descripción no puede superar los 50 caracteres")]
    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
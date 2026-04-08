using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agro_Mercado.AppMVC.Models;

public partial class DetalleCompra
{
    public int Id { get; set; }

    [Required]
    public int CompraId { get; set; }

    [Required]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una presentación")]
    public int ProductoPresentacionId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
    public int Cantidad { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "El precio debe ser mayor que 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SubTotal { get; set; }

    public virtual Compra Compra { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;

    public virtual ProductoPresentacion ProductoPresentacion { get; set; } = null!;
}
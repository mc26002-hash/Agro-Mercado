using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agro_Mercado.AppMVC.Models;

public partial class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Debe seleccionar una categoría")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una unidad de medida")]
    public int UnidadMedidaId { get; set; }

    [Required(ErrorMessage = "El precio de venta es obligatorio")]
    [Range(0.01, 100000, ErrorMessage = "El precio debe ser mayor que 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioVenta { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int? Stock { get; set; }

    public bool? Activo { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
    public int? StockMinimo { get; set; }

    [Display(Name = "Fecha de Registro")]
    [DataType(DataType.Date)]
    public DateTime? FechaRegistro { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PrecioCompraPromedio { get; set; }

    public virtual ICollection<AjusteInventario> AjusteInventarios { get; set; } = new List<AjusteInventario>();

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual ICollection<DetalleVentum> DetalleVenta { get; set; } = new List<DetalleVentum>();

    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; set; } = new List<MovimientosInventario>();

    public virtual UnidadMedidum UnidadMedida { get; set; } = null!;
}
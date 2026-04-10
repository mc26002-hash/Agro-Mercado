using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agro_Mercado.AppMVC.Models;

public partial class Venta
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha de la venta es obligatoria")]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un cliente")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un empleado")]
    public int EmpleadoId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Iva { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [StringLength(20)]
    public string? NumeroFactura { get; set; }

    [StringLength(50)]
    public string? MetodoPago { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaFactura { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;
    public virtual Empleado Empleado { get; set; } = null!;

    // 🔥 RELACIÓN CLAVE
    public virtual ICollection<DetalleVentum> DetalleVenta { get; set; } = new List<DetalleVentum>();
}
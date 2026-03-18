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
    public DateTime? Fecha { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un cliente")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un empleado")]
    public int EmpleadoId { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "El total debe ser mayor que 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Total { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Iva { get; set; }

    [StringLength(20, ErrorMessage = "El número de factura no puede superar los 20 caracteres")]
    public string? NumeroFactura { get; set; }

    [StringLength(50, ErrorMessage = "El método de pago no puede superar los 50 caracteres")]
    public string? MetodoPago { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaFactura { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<DetalleVentum> DetalleVenta { get; set; } = new List<DetalleVentum>();

    public virtual Empleado Empleado { get; set; } = null!;
}
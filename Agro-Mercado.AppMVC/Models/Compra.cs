using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agro_Mercado.AppMVC.Models;

public partial class Compra
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha de la compra es obligatoria")]
    [DataType(DataType.Date)]
    public DateTime? Fecha { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un proveedor")]
    public int ProveedorId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un empleado")]
    public int EmpleadoId { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "El total debe ser mayor que 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Total { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual Empleado Empleado { get; set; } = null!;

    public virtual Proveedore Proveedor { get; set; } = null!;
}
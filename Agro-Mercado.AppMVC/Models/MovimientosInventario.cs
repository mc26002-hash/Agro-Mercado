using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agro_Mercado.AppMVC.Models;

public partial class MovimientosInventario
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    // 🔥 NUEVO
    public int? ProductoPresentacionId { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cantidad { get; set; }

    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio")]
    [StringLength(200)]
    public string Motivo { get; set; } = null!;

    public int? ReferenciaId { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    // 🔥 NUEVO
    public virtual ProductoPresentacion? ProductoPresentacion { get; set; }
}
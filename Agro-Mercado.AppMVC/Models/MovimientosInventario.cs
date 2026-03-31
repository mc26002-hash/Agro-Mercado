using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agro_Mercado.AppMVC.Models;

public partial class MovimientosInventario
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int Cantidad { get; set; }

    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio")]
    [StringLength(200)]
    public string Motivo { get; set; } = null!;

    public int? ReferenciaId { get; set; }

    public virtual Producto Producto { get; set; } = null!;
}

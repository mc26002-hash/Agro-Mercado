using System;
using System.Collections.Generic;

namespace Agro_Mercado.AppMVC.Models;

public partial class Venta
{
    public int Id { get; set; }

    public DateTime? Fecha { get; set; }

    public int ClienteId { get; set; }

    public int EmpleadoId { get; set; }

    public decimal? Total { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? Iva { get; set; }

    public string? NumeroFactura { get; set; }

    public string? MetodoPago { get; set; }

    public DateTime? FechaFactura { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<DetalleVentum> DetalleVenta { get; set; } = new List<DetalleVentum>();

    public virtual Empleado Empleado { get; set; } = null!;
}

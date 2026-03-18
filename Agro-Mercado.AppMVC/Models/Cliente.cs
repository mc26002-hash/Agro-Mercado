using System;
using System.Collections.Generic;

namespace Agro_Mercado.AppMVC.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public bool? Activo { get; set; }

    public string? Nit { get; set; }

    public string? Nrc { get; set; }

    public string? Dui { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}

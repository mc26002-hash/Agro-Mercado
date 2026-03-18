using System;
using System.Collections.Generic;

namespace Agro_Mercado.AppMVC.Models;

public partial class Proveedore
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string Direccion { get; set; } = null!;

    public bool? Activo { get; set; }

    public string? Nit { get; set; }

    public string? Nrc { get; set; }

    public string? CorreoElectronico { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}

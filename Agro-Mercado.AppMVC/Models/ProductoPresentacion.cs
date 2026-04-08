using Agro_Mercado.AppMVC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agro_Mercado.AppMVC.Models
{
    public class ProductoPresentacion
    {
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;
        // Ej: Caja, Saco, Unidad, Libra, Bandeja

        [Required(ErrorMessage = "La equivalencia es obligatoria")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Equivalencia { get; set; }
        // Ej: 100 tomates por caja, 50 libras por saco

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [StringLength(20)]
        public string Tipo { get; set; } = null!;
        // "Compra" o "Venta"

        public bool Activo { get; set; } = true;

        // 🔗 Relación
        public virtual Producto Producto { get; set; } = null!;
    }
}
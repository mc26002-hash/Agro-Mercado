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
        

        [Required(ErrorMessage = "La equivalencia es obligatoria")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Equivalencia { get; set; }
        

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [StringLength(20)]
        public string Tipo { get; set; } = null!;
        

        public bool Activo { get; set; } = true;

        
        public virtual Producto Producto { get; set; } = null!;
    }
}
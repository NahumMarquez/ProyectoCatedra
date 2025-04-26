using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCatedra.Models
{
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VentaId { get; set; }

        [ForeignKey("VentaId")]
        public Venta Venta { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; } // Cambiado a propiedad de lectura/escritura
    }
}
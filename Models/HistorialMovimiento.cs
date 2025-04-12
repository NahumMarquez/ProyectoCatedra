using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCatedra.Models
{
    public class HistorialMovimiento
    {
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoMovimiento { get; set; } // "Entrada", "Salida", "Ajuste"

        [Required]
        public int Cantidad { get; set; }

        public DateTime FechaMovimiento { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string Usuario { get; set; }

        [StringLength(255)]
        public string Comentario { get; set; }
    }
}
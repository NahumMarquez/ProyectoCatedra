using System.ComponentModel.DataAnnotations;
namespace ProyectoCatedra.Models
{
    public class Producto
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }
        public int Ventas { get; set; } = 0;
        [Required]
        public int StockMinimo { get; set; }
        [Required]
        public string Categoria { get; set; }

        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100")]
        public int Descuento { get; set; } = 0; // en porcentaje

    }
}

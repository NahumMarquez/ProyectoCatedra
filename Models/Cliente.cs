using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoCatedra.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Telefono { get; set; }

        public string Direccion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Propiedad de navegación para las ventas del cliente
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
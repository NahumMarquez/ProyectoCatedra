namespace ProyectoCatedra.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total { get; set; }

        // Relación con cliente (puede ser nula para ventas sin registro)
        public int? ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        // Detalles de la venta
        public ICollection<DetalleVenta> Detalles { get; set; }
        public DateTime FechaVenta { get; internal set; }
    }
}
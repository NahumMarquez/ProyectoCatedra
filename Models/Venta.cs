namespace ProyectoCatedra.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public string NombreCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
    }
}

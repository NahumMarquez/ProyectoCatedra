namespace ProyectoCatedra.Models
{
    public class Devolucion
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public Venta Venta { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int CantidadDevuelta { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Motivo { get; set; }
        public decimal MontoReembolsado { get; set; }
    }
}

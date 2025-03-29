namespace Managerproduct.Models
{
    public class Inventario
    {
        public int Id { get; set; }

        public int Productoid { get; set; }

        public Producto Producto { get; set; }

        public int Cantidad { get; set; }
    }
}

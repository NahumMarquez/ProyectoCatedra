namespace Managerproduct.Models
{
    public class Proveedor
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
        //referencia de relacaion con productos uno a muchos

        public List<Producto> Productos { get; set; }=new List<Producto>();
    }
}

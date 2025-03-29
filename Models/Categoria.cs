using System.ComponentModel.DataAnnotations;

namespace Managerproduct.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
        //referencia de relacioncon producto uno a muchos

        public List<Producto> Productos { get; set; } = new List<Producto>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Managerproduct.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        [Column(TypeName = "decimal(18,2)")] // Define la precisión en la BD
        public decimal Precio { get; set; }

        // Relación con categoría
        public int Categoriaid { get; set; }
        public Categoria Categoria { get; set; }

        // Relación con proveedor
        public int Proveedorid { get; set; }
        public Proveedor Proveedor { get; set; }

        // Relación uno a uno con inventario
        public Inventario Inventario { get; set; }
    }
}

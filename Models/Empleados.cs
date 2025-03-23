using System.ComponentModel.DataAnnotations;
namespace ProyectoCatedra.Models
{
    public class Empleados
    {
        [Key]
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public string correo { get; set; }
        public string rol { get; set; }
    }
}

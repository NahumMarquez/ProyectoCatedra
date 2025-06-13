using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iText.Forms.Xfdf;
namespace ProyectoCatedra.Models
{
    public class Empleados
    {
        public int Id { get; set; }
        [Required]
        [Column("usuario")]
        public string Usuario { get; set; }
        [Required]
        [Column("contraseña")]
        public string Contraseña { get; set; }

        [Column("correo")]
        public string Correo { get; set; }

        [Column("rol")]
        public string Rol { get; set; }
        [Column("ultimo_cambio_contraseña")]
        public DateTime UltimoCambioContraseña { get; set; } = DateTime.Now;

        [Column("requiere_cambio_contraseña")]
        public bool RequiereCambioContraseña { get; set; } = false;
    }
}

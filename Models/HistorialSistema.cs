using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoCatedra.Models
{
    public class HistorialSistema
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Usuario { get; set; }

        [Required]
        [StringLength(255)]
        public string Accion { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}


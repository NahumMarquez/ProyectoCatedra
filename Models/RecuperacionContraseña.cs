using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCatedra.Models
{
    public class RecuperacionContraseña
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("correo")]
        public string Email { get; set; }

        [Required]
        [Column("token")]
        public string Token { get; set; }

        [Column("fecha_expiracion")]
        public DateTime Expiracion { get; set; }
    }
}

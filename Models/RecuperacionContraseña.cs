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
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        public DateTime Expiracion { get; set; }
    }
}

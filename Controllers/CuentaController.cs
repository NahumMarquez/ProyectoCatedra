using Microsoft.AspNetCore.Mvc;
using ProyectoCatedra.Models;
using ProyectoCatedra.Db;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoCatedra.Controllers
{
    public class CuentaController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Empleados empleado)
        {
            string usuarioValido = "admin";
            string contrasenaValida = "123456";

            if (empleado.Usuario == usuarioValido && empleado.Contraseña == contrasenaValida)
            {
                return RedirectToAction("Inicio");
            }
            else
            {
                ViewBag.Error = "Credenciales incorrectas";
                return View();
            }
        }

        public IActionResult Inicio()
        {
            return View();
        }
        private readonly AppDbContext _context;

        public object Correo { get; set; }

        public CuentaController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Recuperar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Recuperar(string Email)
        {
            var usuario = _context.Empleados.FirstOrDefault(e => e.Usuario == Email);
            if (usuario == null)
            {
                ViewBag.Mensaje = "El correo ingresado no está registrado.";
                return View();
            }

            string token = GenerarToken();
            _context.RecuperacionContraseñas.Add(new RecuperacionContraseña
            {
                Email = Email,
                Token = token,
                Expiracion = DateTime.Now.AddHours(1)
            });
            _context.SaveChanges();

            // Simulación del envío de correo con enlace de recuperación
            string enlace = Url.Action("Resetear", "Cuenta", new { token }, Request.Scheme);
            Console.WriteLine($"Enviar este enlace al usuario: {enlace}");

            ViewBag.Mensaje = "Se ha enviado un enlace de recuperación a tu correo.";
            return View();
        }

        public IActionResult Resetear(string token)
        {
            var registro = _context.RecuperacionContraseñas.FirstOrDefault(r => r.Token == token && r.Expiracion > DateTime.Now);
            if (registro == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Resetear(string token, string nuevaContrasena)
        {
            var registro = _context.RecuperacionContraseñas.FirstOrDefault(r => r.Token == token && r.Expiracion > DateTime.Now);
            if (registro == null)
            {
                return RedirectToAction("Login");
            }

            var usuario = _context.Empleados.FirstOrDefault(e => e.Usuario == registro.Email);
            if (usuario != null)
            {
                usuario.Contraseña = nuevaContrasena;  // Encriptar antes de guardar en producción
                _context.SaveChanges();
            }

            return RedirectToAction("Login");
        }

        private string GenerarToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenBytes = new byte[16];
                rng.GetBytes(tokenBytes);
                return Convert.ToBase64String(tokenBytes);
            }
        }
    }
}
    

    
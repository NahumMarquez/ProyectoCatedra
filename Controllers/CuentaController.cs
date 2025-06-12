using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCatedra.Db;
using ProyectoCatedra.Models;
using System.Net;
using System.Net.Mail;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ProyectoCatedra.Controllers
{
    public class CuentaController : Controller
    {
        private readonly AppDbContext _context;

        public CuentaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(Empleados empleado)
        {
            if (string.IsNullOrWhiteSpace(empleado.Usuario) || string.IsNullOrWhiteSpace(empleado.Contraseña))
            {
                ViewBag.Error = "Todos los campos son obligatorios.";
                return View();
            }

            var usuario = _context.Empleados
                .FirstOrDefault(e => e.Usuario == empleado.Usuario);

            if (usuario == null)
            {
                ViewBag.Error = "Credenciales incorrectas.";
                return View();
            }

            // Comparar la contraseña directamente (sin encriptar)
            if (usuario.Contraseña != empleado.Contraseña)
            {
                ViewBag.Error = "Credenciales incorrectas.";
                return View();
            }

            // Iniciar sesión con sesión
            HttpContext.Session.SetString("Usuario", usuario.Usuario);
            HttpContext.Session.SetString("Correo", usuario.Correo);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            return RedirectToAction("Inicio");
        }

        // Cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Elimina la sesión
            return RedirectToAction("Login");
        }

        // Página de inicio
        public IActionResult Inicio()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario")))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // GET: Recuperar contraseña
        public IActionResult Recuperar()
        {
            return View();
        }

        // POST: Recuperar contraseña
        [HttpPost]
        public IActionResult Recuperar(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Mensaje = "Debe ingresar un correo.";
                return View();
            }

            string emailNormalizado = email.Trim().ToLower();
            var usuario = _context.Empleados.FirstOrDefault(e => e.Correo.ToLower() == emailNormalizado);

            if (usuario == null)
            {
                ViewBag.Mensaje = "El correo ingresado no está registrado.";
                return View();
            }

            string token = GenerarToken();
            _context.RecuperacionContraseñas.Add(new RecuperacionContraseña
            {
                Email = usuario.Correo,
                Token = token,
                Expiracion = DateTime.Now.AddHours(1)
            });
            _context.SaveChanges();

            string enlace = Url.Action("Resetear", "Cuenta", new { token }, Request.Scheme);
            Console.WriteLine($"Enviar este enlace al usuario: {enlace}");

            try
            {
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("ccabigail48@gmail.com", "vnvl tzoi xdut dewq"),
                    EnableSsl = true
                };

                MailMessage mensaje = new MailMessage
                {
                    From = new MailAddress("ccabigail48@gmail.com"),
                    Subject = "Recuperación de contraseña",
                    Body = $"Haz clic en este enlace para restablecer tu contraseña: {enlace}",
                    IsBodyHtml = true
                };
                mensaje.To.Add(usuario.Correo);

                smtp.Send(mensaje);
                Console.WriteLine("Correo enviado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
                ViewBag.Mensaje = "Hubo un error al enviar el correo.";
                return View();
            }

            ViewBag.Mensaje = "Se ha enviado un enlace de recuperación a tu correo.";
            return View();
        }

        // GET: Resetear contraseña
        public IActionResult Resetear(string token)
        {
            var registro = _context.RecuperacionContraseñas.FirstOrDefault(r => r.Token == token && r.Expiracion > DateTime.Now);
            if (registro == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Token = token;
            return View();
        }

        // POST: Resetear contraseña
        [HttpPost]
        public IActionResult Resetear(string token, string nuevaContrasena)
        {
            var registro = _context.RecuperacionContraseñas.FirstOrDefault(r => r.Token == token && r.Expiracion > DateTime.Now);
            if (registro == null)
            {
                return RedirectToAction("Login");
            }

            var usuario = _context.Empleados.FirstOrDefault(e => e.Correo == registro.Email);
            if (usuario != null)
            {
                usuario.Contraseña = nuevaContrasena; // Guardar sin encriptar
                _context.SaveChanges();
            }

            return RedirectToAction("Login");
        }

        // Generar Token
        private string GenerarToken()
        {
            var rng = new Random();
            byte[] tokenBytes = new byte[16];
            rng.NextBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }

        // GET: Cambiar contraseña (voluntario) - Eliminamos la necesidad de contraseña actual
        public IActionResult CambiarContraseña()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario")))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // POST: Cambiar contraseña (voluntario) - Eliminamos el parámetro contraseñaActual
        [HttpPost]
        public IActionResult CambiarContraseña(string nuevaContraseña, string confirmarContraseña)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario")))
            {
                return RedirectToAction("Login");
            }

            var usuario = _context.Empleados
                .FirstOrDefault(e => e.Usuario == HttpContext.Session.GetString("Usuario"));

            if (usuario == null)
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrWhiteSpace(nuevaContraseña) ||
                string.IsNullOrWhiteSpace(confirmarContraseña))
            {
                ViewBag.Error = "Todos los campos son obligatorios.";
                return View();
            }

            if (nuevaContraseña != confirmarContraseña)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            usuario.Contraseña = nuevaContraseña;
            usuario.UltimoCambioContraseña = DateTime.Now;
            usuario.RequiereCambioContraseña = false;
            _context.SaveChanges();

            ViewBag.Mensaje = "Contraseña cambiada con éxito.";
            return View();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ProyectoCatedra.Db;
using ProyectoCatedra.Models;
using Microsoft.AspNetCore.Mvc;

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
            string contrasenaValida = "1234";

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
    }
}

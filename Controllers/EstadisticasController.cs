using Microsoft.AspNetCore.Mvc;
using ProyectoCatedra.Db;
using ProyectoCatedra.Models;
using System.Linq;

namespace ProyectoCatedra.Controllers
{
    public class EstadisticasController : Controller
    {
        private readonly AppDbContext _context;

        public EstadisticasController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Verificar sesión
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario")))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            // Obtener productos ordenados por ventas
            var productos = _context.Productos.OrderByDescending(p => p.Ventas).ToList();

            // Separar los más y menos vendidos
            var masVendidos = productos.Take(5).ToList();
            var menosVendidos = productos.OrderBy(p => p.Ventas).Take(5).ToList();

            ViewBag.MasVendidos = masVendidos;
            ViewBag.MenosVendidos = menosVendidos;

            return View();
        }

        // Método para registrar una venta (puede ser llamado desde otros controladores)
        public void RegistrarVenta(int productoId, int cantidad)
        {
            var producto = _context.Productos.Find(productoId);
            if (producto != null)
            {
                producto.Ventas += cantidad;
                producto.Cantidad -= cantidad; // Actualizar stock
                _context.SaveChanges();
            }
        }
    }
}
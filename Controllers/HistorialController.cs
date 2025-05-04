using Microsoft.AspNetCore.Mvc;
using ProyectoCatedra.Db;
using ProyectoCatedra.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProyectoCatedra.Controllers
{
    public class HistorialController : Controller
    {
        private readonly AppDbContext _context;

        public HistorialController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Historial/Producto/{id}
        public IActionResult Producto(int id)
        {
            // Verificar sesión
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario")))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var producto = _context.Productos.Find(id);
            if (producto == null)
            {
                return NotFound();
            }

            var historial = _context.HistorialMovimientos
                .Include(h => h.Producto)
                .Where(h => h.ProductoId == id)
                .OrderByDescending(h => h.FechaMovimiento)
                .ToList();

            ViewBag.Producto = producto;
            return View(historial);
        }
    }
}
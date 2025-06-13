using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCatedra.Models;
using ProyectoCatedra.Db;

namespace ProyectoCatedra.Controllers
{
    public class DevolucionController : Controller
    {
        private readonly AppDbContext _context;
        public DevolucionController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Registrar()
        {
            ViewBag.Ventas = _context.Ventas
            .Include(v => v.Detalles)
            .ThenInclude(d => d.Producto)
            .ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Registrar(int ventaId, int productoId, int cantidadDevuelta, string
       motivo)
        {
            var venta = _context.Ventas
            .Include(v => v.Detalles)
            .FirstOrDefault(v => v.Id == ventaId);
            if (venta == null) return NotFound();
            var detalle = venta.Detalles.FirstOrDefault(d => d.ProductoId == productoId);
            if (detalle == null || detalle.Cantidad < cantidadDevuelta) return BadRequest();
            var producto = _context.Productos.Find(productoId);
            producto.Cantidad += cantidadDevuelta;
            var montoReembolsado = detalle.PrecioUnitario * cantidadDevuelta;
            var devolucion = new Devolucion
            {
                VentaId = ventaId,
                ProductoId = productoId,
                CantidadDevuelta = cantidadDevuelta,
                Motivo = motivo,
                MontoReembolsado = montoReembolsado
            };
            _context.Devoluciones.Add(devolucion);
            _context.SaveChanges();
            return RedirectToAction("Confirmacion");
        }
        public IActionResult Confirmacion()
        {
            return View();
        }
    }
}

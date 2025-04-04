using Microsoft.AspNetCore.Mvc;
using ProyectoCatedra.Db;
using ProyectoCatedra.Models;
using System.Linq;
namespace ProyectoCatedra.Controllers
{
    public class InventarioController : Controller
    {
        private readonly AppDbContext _context;
        public InventarioController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var productos = _context.Productos.ToList();
            return View(productos);
        }
        public IActionResult Agregar() => View(new Producto());

        [HttpPost]
        public IActionResult Agregar(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producto);
        }
        public IActionResult Actualizar(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
                return NotFound();
            return View(producto);
        }
        [HttpPost]
        public IActionResult Actualizar(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Update(producto);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producto);
        }
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
                return NotFound();

            _context.Productos.Remove(producto);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult VenderProducto(int id, int cantidad)
        {
            var estadisticasController = new EstadisticasController(_context);
            estadisticasController.RegistrarVenta(id, cantidad);
            return RedirectToAction("Index");
        }

    }
}


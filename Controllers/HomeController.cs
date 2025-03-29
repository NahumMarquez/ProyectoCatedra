using System.Diagnostics;
using Managerproduct.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Managerproduct.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, AplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .ToListAsync();
            return View(productos); // Asegúrate de pasar 'productos' a la vista
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Create()
        {
            ViewBag.Categoriaid = new SelectList(_context.Categorias, "Id", "Nombre");
            ViewBag.Proveedorid = new SelectList(_context.Proveedores, "Id", "Nombre");
            return View(new Producto());
        }



        [HttpPost]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Asegúrate de que esto esté presente
            }
            // Si hay error, recarga los SelectList
            ViewBag.Categoriaid = new SelectList(_context.Categorias, "Id", "Nombre", producto.Categoriaid);
            ViewBag.Proveedorid = new SelectList(_context.Proveedores, "Id", "Nombre", producto.Proveedorid);
            return View(producto);
        }


    }
}

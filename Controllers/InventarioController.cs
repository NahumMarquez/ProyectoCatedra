using Microsoft.AspNetCore.Mvc;
using ProyectoCatedra.Db;
using ProyectoCatedra.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using OfficeOpenXml;
using iText.Forms.Xfdf;


namespace ProyectoCatedra.Controllers
{
    public class InventarioController : Controller
    {
        private readonly AppDbContext _context;
        public InventarioController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string categoria)
        {
            var productos = _context.Productos.ToList();
            var productosBajoStock = productos.Where(p => p.Cantidad <= 10).ToList();

            ViewBag.ProductosBajoStock = productosBajoStock;

            var producto = string.IsNullOrEmpty(categoria)
                ? _context.Productos.ToList()
                : _context.Productos.Where(p => p.Categoria == categoria).ToList();

            ViewBag.Categorias = _context.Productos
                .Select(p => p.Categoria)
                .Distinct()
                .ToList();

            return View(producto);
        }

        // Nuevo método para buscar productos
        [HttpPost]
        public IActionResult Buscar(string criterio)
        {
            // Si el campo de búsqueda está vacío, redirigimos a la página principal
            if (string.IsNullOrWhiteSpace(criterio))
            {
                return RedirectToAction("Index", "Cuenta");
            }
            // Buscamos en la tabla Producto por nombre, código o categoría (ignorando mayúsculas)
            var resultados = _context.Productos
            .Where(p => p.Nombre.Contains(criterio) ||
            p.Categoria.Contains(criterio))
            .ToList();
            // Enviamos los resultados a la vista ResultadosBusqueda
            return View("ResultadosBusqueda", resultados);
        }
        public IActionResult Agregar() => View(new Producto());

        [HttpPost]
        public IActionResult Agregar(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
                RegistrarMovimiento(producto.Id, "Entrada", producto.Cantidad, "Entrada inicial de producto");
                return RedirectToAction("Index");
            }
            return View(producto);
        }
        // Método auxiliar para registrar movimientos
        private void RegistrarMovimiento(int productoId, string tipo, int cantidad, string comentario = "")
        {
            var movimiento = new HistorialMovimiento
            {
                ProductoId = productoId,
                TipoMovimiento = tipo,
                Cantidad = cantidad,
                Usuario = HttpContext.Session.GetString("Usuario") ?? "Sistema",
                Comentario = comentario
            };

            _context.HistorialMovimientos.Add(movimiento);
            _context.SaveChanges();
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
                var productoExistente = _context.Productos.Find(producto.Id);
                if (productoExistente == null)
                    return NotFound();

                // Verificar si hubo cambio en la cantidad
                if (productoExistente.Cantidad != producto.Cantidad)
                {
                    int diferencia = producto.Cantidad - productoExistente.Cantidad;
                    string tipoMovimiento = diferencia > 0 ? "Ajuste (Entrada)" : "Ajuste (Salida)";
                    string comentario = $"Ajuste de inventario. Cantidad anterior: {productoExistente.Cantidad}";

                    RegistrarMovimiento(producto.Id, tipoMovimiento, Math.Abs(diferencia), comentario);
                }

                _context.Entry(productoExistente).CurrentValues.SetValues(producto);
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

        // Método para mostrar el informe en una vista HTML
        public IActionResult Informe()
        {
            var productos = _context.Productos.ToList();
            return View(productos);
        }
        // Método para exportar el informe a PDF
        public IActionResult ExportarPDF()
        {
            var productos = _context.Productos.ToList();
            ExcelPackage.License.SetNonCommercialOrganization("FarmaciaValentina");

            using (var package = new ExcelPackage())
            using (MemoryStream ms = new MemoryStream())
            {
                // 👇 REGISTRO NECESARIO

                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                Paragraph titulo = new Paragraph("Informe de Inventario")
                    .SetFontSize(18)
                    .SetBold();

                document.Add(titulo);

                Table table = new Table(4);
                table.AddHeaderCell("ID");
                table.AddHeaderCell("Nombre");
                table.AddHeaderCell("Cantidad");
                table.AddHeaderCell("Precio");

                foreach (var producto in productos)
                {
                    table.AddCell(producto.Id.ToString());
                    table.AddCell(producto.Nombre);
                    table.AddCell(producto.Cantidad.ToString());
                    table.AddCell($"${producto.Precio}");
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "Informe_Inventario.pdf");
            }
        }

        [HttpGet]
        public IActionResult VenderProducto(int id, int cantidad)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
                return NotFound();

            if (cantidad <= 0 || cantidad > producto.Cantidad)
            {
                TempData["Error"] = "Cantidad no válida";
                return RedirectToAction("GenerarTicket", new { id = id, cantidad = cantidad });
            }

            producto.Cantidad -= cantidad;
            _context.SaveChanges();

            // Registrar movimiento de salida
            RegistrarMovimiento(id, "Salida", cantidad, "Venta de producto");

            var estadisticasController = new EstadisticasController(_context);
            estadisticasController.RegistrarVenta(id, cantidad);

            return RedirectToAction("Index");
        }

        public IActionResult GenerarTicket(int id, int cantidad)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
                return NotFound();

            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph("Ticket de Compra").SetBold().SetFontSize(18));
                document.Add(new Paragraph($"Fecha: {DateTime.Now}"));
                document.Add(new Paragraph($"Producto: {producto.Nombre}"));
                document.Add(new Paragraph($"Cantidad: {cantidad}"));
                document.Add(new Paragraph($"Precio Unitario: ${producto.Precio}"));
                document.Add(new Paragraph($"Total: ${producto.Precio * cantidad}"));
                document.Add(new Paragraph("Gracias por su compra."));

                document.Close();

                return File(ms.ToArray(), "application/pdf", "Ticket_Compra.pdf");
            }
        }
    }
}

    

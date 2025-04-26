using Microsoft.AspNetCore.Mvc;
using ProyectoCatedra.Db;
using ProyectoCatedra.Models;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using OfficeOpenXml;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Bouncycastle;
using OfficeOpenXml.Style;

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
            var productosBajoStock = productos.Where(p => p.Cantidad <= 10).ToList();
            var clientes = _context.Clientes.ToList();

            ViewBag.ProductosBajoStock = productosBajoStock;
            ViewBag.Clientes = clientes;
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

            // Primero eliminar los detalles de venta asociados
            var detalles = _context.DetalleVentas.Where(d => d.ProductoId == id).ToList();
            _context.DetalleVentas.RemoveRange(detalles);

            // Luego eliminar el producto
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

        // Método para exportar el informe a Excel
        public IActionResult ExportarExcel()
        {
            var productos = _context.Productos.ToList();

            // ✅ Establecer licencia correctamente (EPPlus 8)
            OfficeOpenXml.ExcelPackage.License.SetNonCommercialOrganization("Mi Organización");

            using (ExcelPackage excel = new ExcelPackage())
            {
                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Inventario");

                // Encabezados con estilo
                string[] headers = { "ID", "Nombre", "Cantidad", "Precio" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cells[1, i + 1].Value = headers[i];
                    ws.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(31, 78, 121));
                    ws.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    ws.Cells[1, i + 1].Style.Font.Bold = true;
                    ws.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[1, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Datos
                int row = 2;
                foreach (var producto in productos)
                {
                    ws.Cells[row, 1].Value = producto.Id;
                    ws.Cells[row, 2].Value = producto.Nombre;
                    ws.Cells[row, 3].Value = producto.Cantidad;
                    ws.Cells[row, 4].Value = producto.Precio;
                    ws.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";

                    for (int col = 1; col <= 4; col++)
                    {
                        ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    row++;
                }

                ws.Cells.AutoFitColumns();
                var stream = new MemoryStream();
                excel.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inventario.xlsx");
            }
        }

        [HttpGet]
        public IActionResult Actualizar(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        

        [HttpGet]
        public IActionResult VenderProducto(int id, int cantidad, int? clienteId)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.Id == id);
            if (producto == null || cantidad < 1)
            {
                return NotFound();
            }

            if (producto.Cantidad < cantidad)
            {
                TempData["Error"] = "No hay suficiente stock para realizar la venta.";
                return RedirectToAction("Index");
            }

            // Restar la cantidad vendida al producto
            producto.Cantidad -= cantidad;

            var venta = new Venta
            {
                FechaVenta = DateTime.Now,
                ClienteId = clienteId,
                Detalles = new List<DetalleVenta>
        {
            new DetalleVenta
            {
                ProductoId = producto.Id,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio,
                Subtotal = producto.Precio * cantidad
            }
        }
            };

            // Registrar el movimiento de salida
            RegistrarMovimiento(producto.Id, "Venta", cantidad, "Venta realizada");

            _context.Ventas.Add(venta);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditarCliente(int id)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        public IActionResult EditarCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Clientes.Update(cliente);
                _context.SaveChanges();
                TempData["Success"] = "Cliente actualizado exitosamente";
                return RedirectToAction("ListaClientes");
            }
            return View(cliente);
        }

        [HttpGet]
        public IActionResult EliminarCliente(int id)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente == null)
            {
                return NotFound();
            }

            var tieneVentas = _context.Ventas.Any(v => v.ClienteId == id);
            if (tieneVentas)
            {
                TempData["Error"] = "No se puede eliminar el cliente porque tiene ventas asociadas.";
                return RedirectToAction("ListaClientes");
            }

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();
            TempData["Success"] = "Cliente eliminado exitosamente";
            return RedirectToAction("ListaClientes");
        }

        [HttpGet]
        public IActionResult ListaClientes()
        {
            var clientes = _context.Clientes.ToList();
            return View(clientes);
        }

        [HttpGet]
        public IActionResult RegistrarCliente()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Clientes.Add(cliente);
                _context.SaveChanges();
                TempData["Success"] = "Cliente registrado exitosamente";
                return RedirectToAction("ListaClientes");
            }
            return View(cliente);
        }
    }
}

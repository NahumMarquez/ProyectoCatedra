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
using OfficeOpenXml.Style; // si usas estilos más adelante


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

            ViewBag.ProductosBajoStock = productosBajoStock;
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
                    ws.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(31, 78, 121));
                    ws.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    ws.Cells[1, i + 1].Style.Font.Bold = true;
                    ws.Cells[1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[1, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
                        ws.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    row++;
                }

                ws.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                excel.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inventario.xlsx");
            }
        }
        public IActionResult VenderProducto(int id, int cantidad)
        {
            var estadisticasController = new EstadisticasController(_context);
            estadisticasController.RegistrarVenta(id, cantidad);
            return RedirectToAction("Index");
        }
    }
}

    

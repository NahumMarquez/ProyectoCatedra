using Microsoft.AspNetCore.Mvc;
using ProyectoCatedra.Db;
using ProyectoCatedra.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace ProyectoCatedra.Controllers
{
    public class HistorialSistemaController : Controller
    {
        private readonly AppDbContext _context;

        public HistorialSistemaController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string usuario, DateTime? desde, DateTime? hasta)
        {
            var query = _context.HistorialSistema.AsQueryable();

            if (!string.IsNullOrWhiteSpace(usuario))
                query = query.Where(h => h.Usuario.Contains(usuario));

            if (desde.HasValue)
                query = query.Where(h => h.Fecha >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(h => h.Fecha <= hasta.Value);

            ViewBag.FiltroUsuario = usuario;
            ViewBag.FiltroDesde = desde;
            ViewBag.FiltroHasta = hasta;

            return View(query.OrderByDescending(h => h.Fecha).ToList());
        }

        public IActionResult ExportarPDF(string usuario, DateTime? desde, DateTime? hasta)
        {
            var query = _context.HistorialSistema.AsQueryable();

            if (!string.IsNullOrWhiteSpace(usuario))
                query = query.Where(h => h.Usuario.Contains(usuario));

            if (desde.HasValue)
                query = query.Where(h => h.Fecha >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(h => h.Fecha <= hasta.Value);

            var historial = query.OrderByDescending(h => h.Fecha).ToList();

            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph("Historial del Sistema").SetBold().SetFontSize(16));

                Table table = new Table(3);
                table.AddHeaderCell("Fecha");
                table.AddHeaderCell("Usuario");
                table.AddHeaderCell("Acción");

                foreach (var item in historial)
                {
                    table.AddCell(item.Fecha.ToString("g"));
                    table.AddCell(item.Usuario);
                    table.AddCell(item.Accion);
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "Historial_Sistema.pdf");
            }
        }
    }
}


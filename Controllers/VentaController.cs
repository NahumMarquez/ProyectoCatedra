using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using ProyectoCatedra.Models;
using ProyectoCatedra.Db;
public class VentaController : Controller
{
    private readonly AppDbContext _context;
    public VentaController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Crear()
    {
        ViewBag.Productos = _context.Productos.ToList();
        return View();
    }
    [HttpPost]
    public IActionResult Crear(List<int> productoIds, List<int> cantidades, string NombreCliente, string TelefonoCliente)
    {
        var venta = new Venta
        {
            NombreCliente = NombreCliente,
            TelefonoCliente = TelefonoCliente,
            Detalles = new List<DetalleVenta>()
        };

        decimal total = 0;

        for (int i = 0; i < productoIds.Count; i++)
        {
            var producto = _context.Productos.Find(productoIds[i]);

            if (producto != null && producto.Cantidad >= cantidades[i])
            {
                producto.Cantidad -= cantidades[i];

                decimal precioConDescuento = producto.Precio;
                if (producto.Descuento > 0)
                {
                    precioConDescuento = producto.Precio * (1 - (producto.Descuento / 100m));
                }

                var detalle = new DetalleVenta
                {
                    ProductoId = producto.Id,
                    Cantidad = cantidades[i],
                    PrecioUnitario = precioConDescuento
                };

                venta.Detalles.Add(detalle);
                total += precioConDescuento * cantidades[i];
            }
        }

        venta.Total = total;
        _context.Ventas.Add(venta);
        _context.SaveChanges();

        return RedirectToAction("Ticket", new { id = venta.Id });
    }



    public IActionResult Ticket(int id)
    {
        var venta = _context.Ventas
        .Include(v => v.Detalles)
        .ThenInclude(d => d.Producto)
        .FirstOrDefault(v => v.Id == id);
        return View(venta);
    }
    public IActionResult TicketPDF(int id)
    {
        var venta = _context.Ventas
        .Include(v => v.Detalles)
        .ThenInclude(d => d.Producto)
        .FirstOrDefault(v => v.Id == id);
        if (venta == null)
            return NotFound();
        return new ViewAsPdf("Ticket", venta)
        {
            FileName = $"Ticket_{id}.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A6
        };
    }
    public IActionResult ActualizarStock()
    {
        ViewBag.Productos = _context.Productos.ToList();
        return View();
    }
    [HttpPost]
    public IActionResult ActualizarStock(int productoId, int cantidad)
    {
        var producto = _context.Productos.Find(productoId);
        if (producto != null)
        {
            producto.Cantidad += cantidad;
            _context.SaveChanges();
        }
        return RedirectToAction("ActualizarStock");
    }
}


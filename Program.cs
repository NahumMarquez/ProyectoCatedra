using ProyectoCatedra.Db;
using Microsoft.EntityFrameworkCore;
using ProyectoCatedra.Models;

namespace ProyectoCatedra
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Agregar servicios al contenedor
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddSession();

            var app = builder.Build(); // ✅ Se llama solo una vez

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!context.Empleados.Any()) // Verifica si la tabla está vacía
                {
                    context.Empleados.Add(new Empleados
                    {
                        Usuario = "JefeFarmacia",
                        Correo = "ccabigail48@gmail.com",
                        Contraseña = "abigail", // Considera encriptar la contraseña
                        Rol = "jefe"
                    });
                    context.SaveChanges();
                    Console.WriteLine("👤 Usuario 'admin' agregado a la base de datos.");
                }
            }
            // Configurar el middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Cuenta}/{action=Login}/{id?}");

            app.Run();
        }
    }
}

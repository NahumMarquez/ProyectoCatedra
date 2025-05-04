using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoCatedra.Migrations
{
    /// <inheritdoc />
    public partial class NewCampoDescuentoEnProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Descuento",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "Productos");
        }
    }
}

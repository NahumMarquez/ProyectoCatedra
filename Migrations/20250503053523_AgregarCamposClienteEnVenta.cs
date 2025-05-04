using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoCatedra.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposClienteEnVenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombreCliente",
                table: "Ventas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TelefonoCliente",
                table: "Ventas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreCliente",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "TelefonoCliente",
                table: "Ventas");
        }
    }
}

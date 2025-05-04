using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoCatedra.Migrations
{
    /// <inheritdoc />
    public partial class NewModelHistorial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorialMovimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialMovimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_ProductoId",
                table: "HistorialMovimientos",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialMovimientos");
        }
    }
}

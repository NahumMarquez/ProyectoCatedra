using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoCatedra.Migrations
{
    /// <inheritdoc />
    public partial class CorregirColumnas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "RecuperacionContraseñas",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Expiracion",
                table: "RecuperacionContraseñas",
                newName: "fecha_expiracion");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "RecuperacionContraseñas",
                newName: "correo");

            migrationBuilder.RenameColumn(
                name: "Usuario",
                table: "Empleados",
                newName: "usuario");

            migrationBuilder.RenameColumn(
                name: "Rol",
                table: "Empleados",
                newName: "rol");

            migrationBuilder.RenameColumn(
                name: "Correo",
                table: "Empleados",
                newName: "correo");

            migrationBuilder.RenameColumn(
                name: "Contraseña",
                table: "Empleados",
                newName: "contraseña");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "token",
                table: "RecuperacionContraseñas",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "fecha_expiracion",
                table: "RecuperacionContraseñas",
                newName: "Expiracion");

            migrationBuilder.RenameColumn(
                name: "correo",
                table: "RecuperacionContraseñas",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "usuario",
                table: "Empleados",
                newName: "Usuario");

            migrationBuilder.RenameColumn(
                name: "rol",
                table: "Empleados",
                newName: "Rol");

            migrationBuilder.RenameColumn(
                name: "correo",
                table: "Empleados",
                newName: "Correo");

            migrationBuilder.RenameColumn(
                name: "contraseña",
                table: "Empleados",
                newName: "Contraseña");
        }
    }
}

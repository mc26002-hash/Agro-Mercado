using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agro_Mercado.AppMVC.Migrations
{
    public partial class AjusteVenta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 🔹 TABLA VENTAS

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SubTotal",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Iva", // 🔥 IMPORTANTE: mismo nombre que el modelo
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);


            // 🔹 TABLA DETALLEVENTA

            migrationBuilder.AlterColumn<decimal>(
                name: "SubTotal",
                table: "DetalleVenta",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 🔹 TABLA VENTAS

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SubTotal",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Iva",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            // 🔹 TABLA DETALLEVENTA

            migrationBuilder.AlterColumn<decimal>(
                name: "SubTotal",
                table: "DetalleVenta",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
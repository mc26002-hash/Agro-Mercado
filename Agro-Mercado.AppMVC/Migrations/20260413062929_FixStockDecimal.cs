using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agro_Mercado.AppMVC.Migrations
{
    public partial class FixStockDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 🔥 STOCK
            migrationBuilder.AlterColumn<decimal>(
                name: "Stock",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // 🔥 PRECIO VENTA
            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioVenta",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 🔥 PRECIO COMPRA PROMEDIO
            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioCompraPromedio",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 🔙 STOCK
            migrationBuilder.AlterColumn<int>(
                name: "Stock",
                table: "Productos",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            // 🔙 PRECIO VENTA
            migrationBuilder.AlterColumn<int>(
                name: "PrecioVenta",
                table: "Productos",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            // 🔙 PRECIO COMPRA PROMEDIO
            migrationBuilder.AlterColumn<int>(
                name: "PrecioCompraPromedio",
                table: "Productos",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
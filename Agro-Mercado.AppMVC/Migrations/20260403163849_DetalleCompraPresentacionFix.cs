using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agro_Mercado.AppMVC.Migrations
{
    public partial class DetalleCompraPresentacionFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 🔹 1. Crear columna
            migrationBuilder.AddColumn<int>(
                name: "ProductoPresentacionId",
                table: "DetalleCompra",
                type: "int",
                nullable: true); // 👈 IMPORTANTE

            // 🔹 2. Crear índice
            migrationBuilder.CreateIndex(
                name: "IX_DetalleCompra_ProductoPresentacionId",
                table: "DetalleCompra",
                column: "ProductoPresentacionId");

            // 🔹 3. Crear relación FK
            migrationBuilder.AddForeignKey(
                name: "FK_DetalleCompra_ProductoPresentaciones_ProductoPresentacionId",
                table: "DetalleCompra",
                column: "ProductoPresentacionId",
                principalTable: "ProductoPresentaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // 👈 clave
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleCompra_ProductoPresentaciones_ProductoPresentacionId",
                table: "DetalleCompra");

            migrationBuilder.DropIndex(
                name: "IX_DetalleCompra_ProductoPresentacionId",
                table: "DetalleCompra");

            migrationBuilder.DropColumn(
                name: "ProductoPresentacionId",
                table: "DetalleCompra");
        }
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agro_Mercado.AppMVC.Migrations
{
    public partial class AddProductoPresentacionDetalleVenta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductoPresentacionId",
                table: "DetalleVenta",
                type: "int",
                nullable: true); // 🔥 CAMBIO CLAVE

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVenta_ProductoPresentacionId",
                table: "DetalleVenta",
                column: "ProductoPresentacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleVenta_ProductoPresentaciones_ProductoPresentacionId",
                table: "DetalleVenta",
                column: "ProductoPresentacionId",
                principalTable: "ProductoPresentaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // 🔥 CAMBIO CLAVE
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleVenta_ProductoPresentaciones_ProductoPresentacionId",
                table: "DetalleVenta");

            migrationBuilder.DropIndex(
                name: "IX_DetalleVenta_ProductoPresentacionId",
                table: "DetalleVenta");

            migrationBuilder.DropColumn(
                name: "ProductoPresentacionId",
                table: "DetalleVenta");
        }
    }
}
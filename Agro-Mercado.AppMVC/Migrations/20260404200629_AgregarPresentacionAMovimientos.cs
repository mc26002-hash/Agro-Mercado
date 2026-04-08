using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agro_Mercado.AppMVC.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPresentacionAMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductoPresentacionId",
                table: "MovimientosInventario",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_ProductoPresentacionId",
                table: "MovimientosInventario",
                column: "ProductoPresentacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosInventario_ProductoPresentaciones_ProductoPresentacionId",
                table: "MovimientosInventario",
                column: "ProductoPresentacionId",
                principalTable: "ProductoPresentaciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosInventario_ProductoPresentaciones_ProductoPresentacionId",
                table: "MovimientosInventario");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_ProductoPresentacionId",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "ProductoPresentacionId",
                table: "MovimientosInventario");
        }
    }
}

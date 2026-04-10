using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agro_Mercado.AppMVC.Migrations
{
    /// <inheritdoc />
    public partial class CrearProductoPresentaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductoPresentaciones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    ProductoId = table.Column<int>(nullable: false),

                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),

                    Equivalencia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),

                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),

                    Activo = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoPresentaciones", x => x.Id);

                    table.ForeignKey(
                        name: "FK_ProductoPresentaciones_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoPresentaciones_ProductoId",
                table: "ProductoPresentaciones",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoPresentaciones");
        }
    }
}
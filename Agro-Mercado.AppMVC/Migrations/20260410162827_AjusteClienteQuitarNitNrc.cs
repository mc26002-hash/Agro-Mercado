using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agro_Mercado.AppMVC.Migrations
{
    /// <inheritdoc />
    public partial class AjusteClienteQuitarNitNrc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NIT",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "NRC",
                table: "Clientes");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true)
                .Annotation("Relational:DefaultConstraintName", "DF_Clientes_Activo")
                .OldAnnotation("Relational:DefaultConstraintName", "DF_Clientes_Activo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Clientes",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true)
                .Annotation("Relational:DefaultConstraintName", "DF_Clientes_Activo")
                .OldAnnotation("Relational:DefaultConstraintName", "DF_Clientes_Activo");

            migrationBuilder.AddColumn<string>(
                name: "NIT",
                table: "Clientes",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NRC",
                table: "Clientes",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);
        }
    }
}

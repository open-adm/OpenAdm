using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenAdm.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrdemExibicaoMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdemExibixao",
                table: "Tamanhos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrdemExibixao",
                table: "Produtos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrdemExibixao",
                table: "Pesos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrdemExibixao",
                table: "Categorias",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE \"Tamanhos\" SET \"OrdemExibixao\" = \"Numero\"");
            migrationBuilder.Sql("UPDATE \"Produtos\" SET \"OrdemExibixao\" = \"Numero\"");
            migrationBuilder.Sql("UPDATE \"Pesos\" SET \"OrdemExibixao\" = \"Numero\"");
            migrationBuilder.Sql("UPDATE \"Categorias\" SET \"OrdemExibixao\" = \"Numero\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrdemExibixao",
                table: "Tamanhos");

            migrationBuilder.DropColumn(
                name: "OrdemExibixao",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "OrdemExibixao",
                table: "Pesos");

            migrationBuilder.DropColumn(
                name: "OrdemExibixao",
                table: "Categorias");
        }
    }
}

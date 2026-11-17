using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenAdm.Data.Migrations;

/// <inheritdoc />
public partial class _20261114214307_AjusteKitMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_KitsTabelasPreco_KitId",
            table: "KitsTabelasPreco");

        migrationBuilder.AddColumn<string>(
            name: "NomeImagem",
            table: "Kits",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_KitsTabelasPreco_KitId_TabelaPrecoId",
            table: "KitsTabelasPreco",
            columns: new[] { "KitId", "TabelaPrecoId" },
            unique: true,
            filter: "\"Ativo\" = true");

        migrationBuilder.CreateIndex(
            name: "IX_Kits_Nome",
            table: "Kits",
            column: "Nome",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_KitsTabelasPreco_KitId_TabelaPrecoId",
            table: "KitsTabelasPreco");

        migrationBuilder.DropIndex(
            name: "IX_Kits_Nome",
            table: "Kits");

        migrationBuilder.DropColumn(
            name: "NomeImagem",
            table: "Kits");

        migrationBuilder.CreateIndex(
            name: "IX_KitsTabelasPreco_KitId",
            table: "KitsTabelasPreco",
            column: "KitId");
    }
}

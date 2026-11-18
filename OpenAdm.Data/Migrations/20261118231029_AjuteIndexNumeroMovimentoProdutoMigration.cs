using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenAdm.Data.Migrations;

/// <inheritdoc />
public partial class _20261118231029_AjuteIndexNumeroMovimentoProdutoMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Pedidos_Numero",
            table: "Pedidos",
            column: "Numero");

        migrationBuilder.CreateIndex(
            name: "IX_MovimentacoesDeProdutos_Numero",
            table: "MovimentacoesDeProdutos",
            column: "Numero");

        migrationBuilder.CreateIndex(
            name: "IX_Estoques_Numero",
            table: "Estoques",
            column: "Numero");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Pedidos_Numero",
            table: "Pedidos");

        migrationBuilder.DropIndex(
            name: "IX_MovimentacoesDeProdutos_Numero",
            table: "MovimentacoesDeProdutos");

        migrationBuilder.DropIndex(
            name: "IX_Estoques_Numero",
            table: "Estoques");
    }
}

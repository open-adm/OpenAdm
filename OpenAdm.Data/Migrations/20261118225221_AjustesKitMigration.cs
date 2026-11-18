using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OpenAdm.Data.Migrations;

/// <inheritdoc />
public partial class _20261118225221_AjustesKitMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "KitsItens");

        migrationBuilder.DropTable(
            name: "KitsTabelasPreco");

        migrationBuilder.DropTable(
            name: "Kits");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Kits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                DataDeAtualizacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                DataDeCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Imagem = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                NomeImagem = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Numero = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                VenderPessoaFisica = table.Column<bool>(type: "boolean", nullable: false),
                VenderPessoaJuridica = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Kits", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "KitsItens",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                KitId = table.Column<Guid>(type: "uuid", nullable: false),
                PesoId = table.Column<Guid>(type: "uuid", nullable: true),
                ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                TamanhoId = table.Column<Guid>(type: "uuid", nullable: true),
                Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                Quantidade = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_KitsItens", x => x.Id);
                table.ForeignKey(
                    name: "FK_KitsItens_Kits_KitId",
                    column: x => x.KitId,
                    principalTable: "Kits",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_KitsItens_Pesos_PesoId",
                    column: x => x.PesoId,
                    principalTable: "Pesos",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_KitsItens_Produtos_ProdutoId",
                    column: x => x.ProdutoId,
                    principalTable: "Produtos",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_KitsItens_Tamanhos_TamanhoId",
                    column: x => x.TamanhoId,
                    principalTable: "Tamanhos",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "KitsTabelasPreco",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                KitId = table.Column<Guid>(type: "uuid", nullable: false),
                TabelaPrecoId = table.Column<Guid>(type: "uuid", nullable: false),
                Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                PrecoPessoaFisica = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                PrecoPessoaJuridica = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_KitsTabelasPreco", x => x.Id);
                table.ForeignKey(
                    name: "FK_KitsTabelasPreco_Kits_KitId",
                    column: x => x.KitId,
                    principalTable: "Kits",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_KitsTabelasPreco_TabelaDePreco_TabelaPrecoId",
                    column: x => x.TabelaPrecoId,
                    principalTable: "TabelaDePreco",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Kits_Nome",
            table: "Kits",
            column: "Nome",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_KitsItens_KitId_ProdutoId_TamanhoId_PesoId",
            table: "KitsItens",
            columns: new[] { "KitId", "ProdutoId", "TamanhoId", "PesoId" },
            unique: true,
            filter: "\"Ativo\" = true")
            .Annotation("Npgsql:NullsDistinct", false);

        migrationBuilder.CreateIndex(
            name: "IX_KitsItens_PesoId",
            table: "KitsItens",
            column: "PesoId");

        migrationBuilder.CreateIndex(
            name: "IX_KitsItens_ProdutoId",
            table: "KitsItens",
            column: "ProdutoId");

        migrationBuilder.CreateIndex(
            name: "IX_KitsItens_TamanhoId",
            table: "KitsItens",
            column: "TamanhoId");

        migrationBuilder.CreateIndex(
            name: "IX_KitsTabelasPreco_KitId_TabelaPrecoId",
            table: "KitsTabelasPreco",
            columns: new[] { "KitId", "TabelaPrecoId" },
            unique: true,
            filter: "\"Ativo\" = true");

        migrationBuilder.CreateIndex(
            name: "IX_KitsTabelasPreco_TabelaPrecoId",
            table: "KitsTabelasPreco",
            column: "TabelaPrecoId");
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenAdm.Data.Migrations;

/// <inheritdoc />
public partial class _20261118232409_AvisoUsuarioMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AvisosUsuarios",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                Mensagem = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                DataDeCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                DataDeVisualizacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AvisosUsuarios", x => x.Id);
                table.ForeignKey(
                    name: "FK_AvisosUsuarios_Usuarios_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AvisosUsuarios_UsuarioId_DataDeCriacao_Id",
            table: "AvisosUsuarios",
            columns: new[] { "UsuarioId", "DataDeCriacao", "Id" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AvisosUsuarios");
    }
}

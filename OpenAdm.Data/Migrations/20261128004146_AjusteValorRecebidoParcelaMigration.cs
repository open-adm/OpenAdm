using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenAdm.Data.Migrations;

/// <inheritdoc />
public partial class _20261128004146_AjusteValorRecebidoParcelaMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "DescontoConcedido",
            table: "Parcelas",
            type: "numeric(14,2)",
            precision: 14,
            scale: 2,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "JurosPagosRecebidos",
            table: "Parcelas",
            type: "numeric(14,2)",
            precision: 14,
            scale: 2,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "ValorPagoRecebido",
            table: "Parcelas",
            type: "numeric(14,2)",
            precision: 14,
            scale: 2,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Parcelas",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.Sql("""
            WITH movimentos AS (
                SELECT
                    p."Id" AS parcela_id,
                    CASE
                        -- Contas a pagar: saída soma, entrada subtrai.
                        WHEN p."Tipo" = 1 AND t."TipoTransacaoFinanceira" = 2 THEN 1
                        -- Demais tipos: entrada soma, saída subtrai.
                        WHEN p."Tipo" <> 1 AND t."TipoTransacaoFinanceira" = 1 THEN 1
                        ELSE -1
                    END AS sinal,
                    t."Valor" AS valor,
                    COALESCE(t."Desconto", 0) AS desconto,
                    COALESCE(t."Juros", 0) AS juros
                FROM "Parcelas" AS p
                INNER JOIN "TransacoesFinanceiras" AS t ON t."ParcelaId" = p."Id"
                -- O original e o movimento inverso entram no cálculo do estorno.
                WHERE t."TipoTransacaoFinanceira" IN (1, 2)
            ),
            totais AS (
                SELECT
                    parcela_id,
                    SUM(sinal * valor) AS valor_pago_recebido,
                    SUM(sinal * desconto) AS desconto_concedido,
                    SUM(sinal * juros) AS juros_pagos_recebidos
                FROM movimentos
                GROUP BY parcela_id
            )
            UPDATE "Parcelas" AS p
            SET
                "ValorPagoRecebido" = t.valor_pago_recebido,
                "DescontoConcedido" = t.desconto_concedido,
                "JurosPagosRecebidos" = t.juros_pagos_recebidos
            FROM totais AS t
            WHERE p."Id" = t.parcela_id
              AND (
                  p."ValorPagoRecebido",
                  p."DescontoConcedido",
                  p."JurosPagosRecebidos"
              ) IS DISTINCT FROM (
                  t.valor_pago_recebido,
                  t.desconto_concedido,
                  t.juros_pagos_recebidos
              );
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DescontoConcedido",
            table: "Parcelas");

        migrationBuilder.DropColumn(
            name: "JurosPagosRecebidos",
            table: "Parcelas");

        migrationBuilder.DropColumn(
            name: "ValorPagoRecebido",
            table: "Parcelas");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "Parcelas");
    }
}

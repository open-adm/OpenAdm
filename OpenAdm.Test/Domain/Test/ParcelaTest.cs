using OpenAdm.Domain.Enuns;
using OpenAdm.Domain.Exceptions;
using OpenAdm.Test.Domain.Builder;

namespace OpenAdm.Test.Domain.Test;

public sealed class ParcelaTest
{
    [Theory]
    [InlineData(100, null, null, 0, true)]
    [InlineData(90, 10, null, 0, true)]
    [InlineData(105, null, 5, 0, true)]
    [InlineData(95, 10, 5, 0, true)]
    [InlineData(100, null, 5, 5, false)]
    [InlineData(40, 10, 5, 55, false)]
    public void DeveRegistrarValorMovimentadoEApurarSaldoAoPagar(
        int valor, int? desconto, int? juros, int saldo, bool quitada)
    {
        var parcela = ParcelaBuilder.Init().Build();

        var transacao = parcela.Pagar(valor, null, null, null, desconto, juros);

        Assert.Equal(parcela.Id, transacao.ParcelaId);
        Assert.Equal(TipoTransacaoFinanceiraEnum.Entrada, transacao.TipoTransacaoFinanceira);
        Assert.Equal(valor, transacao.Valor);
        Assert.Equal((decimal?)desconto, transacao.Desconto);
        Assert.Equal((decimal?)juros, transacao.Juros);
        Assert.Equal(valor, parcela.ValorPagoRecebido);
        Assert.Equal(desconto ?? 0, parcela.DescontoConcedido);
        Assert.Equal(juros ?? 0, parcela.JurosPagosRecebidos);
        Assert.Equal(saldo, parcela.ValorAPagarAReceber);
        Assert.Equal(quitada, parcela.Quitada);
    }

    [Theory]
    [InlineData(TipoFaturaEnum.AReceber, 100, null, null, TipoTransacaoFinanceiraEnum.Entrada, TipoTransacaoFinanceiraEnum.Saida)]
    [InlineData(TipoFaturaEnum.AReceber, 90, 10, null, TipoTransacaoFinanceiraEnum.Entrada, TipoTransacaoFinanceiraEnum.Saida)]
    [InlineData(TipoFaturaEnum.APagar, 105, null, 5, TipoTransacaoFinanceiraEnum.Saida, TipoTransacaoFinanceiraEnum.Entrada)]
    [InlineData(TipoFaturaEnum.APagar, 40, 10, 5, TipoTransacaoFinanceiraEnum.Saida, TipoTransacaoFinanceiraEnum.Entrada)]
    public void DeveGerarMovimentoInversoEZerarAcumuladosAoEstornar(
        TipoFaturaEnum tipo, int valor, int? desconto, int? juros,
        TipoTransacaoFinanceiraEnum tipoPagamento, TipoTransacaoFinanceiraEnum tipoEstorno)
    {
        var parcela = ParcelaBuilder.Init().ComTipo(tipo).Build();
        var pagamento = parcela.Pagar(valor, null, null, null, desconto, juros);
        parcela.Transacoes = [pagamento];

        var estorno = Assert.Single(parcela.Estornar());

        Assert.Equal(tipoPagamento, pagamento.TipoTransacaoFinanceira);
        Assert.True(pagamento.FoiEstornado);
        Assert.Equal(parcela.Id, estorno.ParcelaId);
        Assert.Equal(tipoEstorno, estorno.TipoTransacaoFinanceira);
        Assert.Equal(valor, estorno.Valor);
        Assert.Equal((decimal?)desconto, estorno.Desconto);
        Assert.Equal((decimal?)juros, estorno.Juros);
        Assert.Equal(0m, parcela.ValorPagoRecebido);
        Assert.Equal(0m, parcela.DescontoConcedido);
        Assert.Equal(0m, parcela.JurosPagosRecebidos);
        Assert.Equal(parcela.Valor, parcela.ValorAPagarAReceber);
        Assert.False(parcela.Quitada);

        parcela.Transacoes.Add(estorno);
        Assert.Throws<ExceptionApi>(() => parcela.Estornar());
    }

    [Fact]
    public void DeveAcumularBaixasComDescontoEJurosEReverterTotaisNoEstorno()
    {
        var parcela = ParcelaBuilder.Init().Build();

        var primeiraBaixa = parcela.Pagar(40m, null, null, null, 5m, 2m);

        Assert.Equal(40m, parcela.ValorPagoRecebido);
        Assert.Equal(57m, parcela.ValorAPagarAReceber);
        Assert.False(parcela.Quitada);

        var segundaBaixa = parcela.Pagar(55m, null, null, null, 2m, null);

        Assert.Equal(95m, parcela.ValorPagoRecebido);
        Assert.Equal(0m, parcela.ValorAPagarAReceber);
        Assert.Equal(7m, parcela.DescontoConcedido);
        Assert.Equal(2m, parcela.JurosPagosRecebidos);
        Assert.True(parcela.Quitada);

        parcela.Transacoes = [primeiraBaixa, segundaBaixa];

        Assert.Equal(2, parcela.Estornar().Count);
        Assert.Equal(0m, parcela.ValorPagoRecebido);
        Assert.Equal(0m, parcela.DescontoConcedido);
        Assert.Equal(0m, parcela.JurosPagosRecebidos);
        Assert.Equal(100m, parcela.ValorAPagarAReceber);
        Assert.False(parcela.Quitada);
    }

    [Fact]
    public void NaoDeveRealizarOperacoesEmParcelaInativa()
    {
        var parcela = ParcelaBuilder.Init().Build();
        parcela.Inativar();

        Assert.Throws<ExceptionApi>(() => parcela.Edit(
            DateTime.UtcNow.AddDays(10), null, 50, null, null));
        Assert.Throws<ExceptionApi>(() => parcela.Pagar(
            50, null, null, DateTime.UtcNow, null, null));
        Assert.Throws<ExceptionApi>(() => parcela.Estornar());
        Assert.Throws<ExceptionApi>(() => parcela.ConsolidarBaixaParcial());
        Assert.Throws<ExceptionApi>(() => parcela.Inativar());
    }
}

using OpenAdm.Domain.Entities.Bases;
using OpenAdm.Domain.Enuns;
using OpenAdm.Domain.Exceptions;
using OpenAdm.Domain.Extensions;

namespace OpenAdm.Domain.Entities;

public sealed class Parcela : BaseEntity
{
    public Parcela(
        Guid id,
        DateTime dataDeCriacao,
        DateTime dataDeAtualizacao,
        long numero,
        DateTime dataDeVencimento,
        int numeroDaParcela,
        MeioDePagamentoEnum? meioDePagamento,
        decimal valor,
        string? observacao,
        Guid faturaId,
        string? idExterno,
        decimal? desconto,
        TipoFaturaEnum tipo,
        bool quitada,
        decimal? juros,
        bool ativo = true)
        : base(id, dataDeCriacao, dataDeAtualizacao, numero)
    {
        DataDeVencimento = dataDeVencimento;
        NumeroDaParcela = numeroDaParcela;
        MeioDePagamento = meioDePagamento;
        Valor = valor;
        Observacao = observacao;
        FaturaId = faturaId;
        IdExterno = idExterno;
        Desconto = desconto;
        Tipo = tipo;
        Quitada = quitada;
        Juros = juros;
        Ativo = ativo;
    }

    public DateTime DataDeVencimento { get; private set; }
    public int NumeroDaParcela { get; private set; }
    public MeioDePagamentoEnum? MeioDePagamento { get; private set; }
    public decimal Valor { get; private set; }
    public decimal? Desconto { get; private set; }
    public decimal? Juros { get; private set; }
    public string? Observacao { get; private set; }
    public string? IdExterno { get; private set; }
    public TipoFaturaEnum Tipo { get; private set; }
    public Guid FaturaId { get; private set; }
    public bool Quitada { get; private set; }
    public bool Ativo { get; private set; }
    public uint Versao { get; private set; }

    public void Inativar()
    {
        ValidarAtiva();
        Ativo = false;
        DataDeAtualizacao = DateTime.UtcNow;
    }

    public void ConsolidarBaixaParcial()
    {
        ValidarAtiva();
        Valor = ValorAbatido.ArredondarCentavos();
        Quitada = true;
        DataDeAtualizacao = DateTime.UtcNow;
    }

    public StatusParcelaEnum Status
    {
        get
        {
            if (Quitada)
            {
                return StatusParcelaEnum.Pago;
            }
            
            if (Vencida)
            {
                return StatusParcelaEnum.Vencida;
            }

            if (ValorPagoRecebido > 0)
            {
                return StatusParcelaEnum.PagoParcial;
            }

            return StatusParcelaEnum.Pendente;
        }
    }

    public Fatura Fatura { get; set; } = null!;

    public decimal ValorAPagarAReceber => Valor - ValorAbatido;
    public decimal ValorAbatido => ValorPagoRecebido + DescontoConcedido - JurosPagosRecebidos;

    public decimal ValorPagoRecebido { get; private set; }
    public decimal DescontoConcedido { get; private set; }
    public decimal JurosPagosRecebidos { get; private set; }

    public bool Vencida
    {
        get => Quitada ? false : DataDeVencimento.Date < DateTime.UtcNow.Date;
    }

    public IList<TransacaoFinanceira>? Transacoes { get; set; }

    public void Edit(
        DateTime dataDeVencimento,
        MeioDePagamentoEnum? meioDePagamento,
        decimal valor,
        decimal? desconto,
        string? observacao)
    {
        ValidarAtiva();
        DataDeVencimento = dataDeVencimento;
        MeioDePagamento = meioDePagamento;
        Valor = valor;
        Desconto = desconto;
        Observacao = observacao;
    }

    public void AlterarValorAReceber(decimal valorAReceber)
    {
        ValidarAtiva();
        var valor = ValorAbatido + valorAReceber;
        if (valor < ValorAbatido)
            throw new ExceptionApi("O valor da parcela não pode ficar abaixo do valor recebido!");

        Valor = valor.ArredondarCentavos();
        Quitada = ValorAbatido >= Valor;
        DataDeAtualizacao = DateTime.UtcNow;
    }

    public TransacaoFinanceira Pagar(
        decimal valor,
        MeioDePagamentoEnum? meioDePagamento,
        string? observacao,
        DateTime? dataDePagamento,
        decimal? desconto,
        decimal? juros)
    {
        ValidarAtiva();

        if (Quitada)
        {
            throw new ExceptionApi($"A parcela: {NumeroDaParcela} já se encontra paga");
        }

        var transacao = TransacaoFinanceira.NovaTransacao(
            parcelaId: Id,
            dataDeEfetivacao: dataDePagamento,
            valor: valor.ArredondarCentavos(),
            tipoTransacaoFinanceira: Tipo == TipoFaturaEnum.APagar
                ? TipoTransacaoFinanceiraEnum.Saida
                : TipoTransacaoFinanceiraEnum.Entrada,
            meioDePagamento: meioDePagamento,
            observacao: observacao ?? $"Pagamento da parcela: {NumeroDaParcela}",
            foiEstornado: false,
            juros: juros?.ArredondarCentavos(),
            desconto: desconto?.ArredondarCentavos());

        ValorPagoRecebido += transacao.Valor;
        DescontoConcedido += transacao.Desconto ?? 0;
        JurosPagosRecebidos += transacao.Juros ?? 0;
        Quitada = ValorAbatido >= Valor;
        DataDeAtualizacao = DateTime.UtcNow;

        return transacao;
    }

    public IList<TransacaoFinanceira> Estornar()
    {
        ValidarAtiva();

        var tipoTransacaoDePagamento = Tipo == TipoFaturaEnum.APagar
            ? TipoTransacaoFinanceiraEnum.Saida
            : TipoTransacaoFinanceiraEnum.Entrada;

        var transacoesParaEstornar = Transacoes?
            .Where(x => x.FoiEstornado != true &&
                        x.TipoTransacaoFinanceira == tipoTransacaoDePagamento)
            .ToList() ?? [];

        if (transacoesParaEstornar.Count == 0)
        {
            throw new ExceptionApi("Não há transações para estornar!");
        }

        var estornos = new List<TransacaoFinanceira>();

        foreach (var transacao in transacoesParaEstornar)
        {
            var estorno = transacao.Estornar();
            estornos.Add(estorno);
        }

        ValorPagoRecebido = 0;
        DescontoConcedido = 0;
        JurosPagosRecebidos = 0;
        Quitada = false;
        DataDeAtualizacao = DateTime.UtcNow;

        return estornos;
    }

    private void ValidarAtiva()
    {
        if (!Ativo)
            throw new ExceptionApi($"Não é possível realizar operações na parcela inativa: {NumeroDaParcela}!");
    }

    public static Parcela NovaFatura(
        DateTime dataDeVencimento,
        int numeroDaParcela,
        MeioDePagamentoEnum? meioDePagamento,
        decimal valor,
        string? observacao,
        Guid faturaId,
        string? idExterno,
        decimal? desconto,
        decimal? juros,
        TipoFaturaEnum tipoFatura
    )
    {
        return new Parcela(
            id: Guid.NewGuid(),
            dataDeCriacao: DateTime.UtcNow,
            dataDeAtualizacao: DateTime.UtcNow,
            numero: 0,
            dataDeVencimento: dataDeVencimento,
            numeroDaParcela: numeroDaParcela,
            meioDePagamento: meioDePagamento,
            valor: valor,
            observacao: observacao,
            faturaId: faturaId,
            idExterno: idExterno,
            desconto: desconto,
            tipo: tipoFatura,
            quitada: false,
            juros: juros);
    }
}

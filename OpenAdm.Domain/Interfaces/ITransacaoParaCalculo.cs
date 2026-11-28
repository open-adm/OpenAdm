using OpenAdm.Domain.Enuns;

namespace OpenAdm.Domain.Interfaces;

public interface ITransacaoParaCalculo
{
    TipoTransacaoFinanceiraEnum TipoTransacaoFinanceira { get; }
    decimal Valor { get; }
    decimal? Desconto { get; }
    decimal? Juros { get; }
}
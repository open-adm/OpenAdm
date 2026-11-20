using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Model;

namespace OpenAdm.Domain.Interfaces;

public interface IMovimentacaoDeProdutoRepository : IGenericRepository<MovimentacaoDeProduto>
{
    Task AddRangeAsync(IList<MovimentacaoDeProduto> movimentacaoDeProdutos);
    Task<List<MovimentacaoMensalCategoriaModel>> ObterSaidasPorMesECategoriaAsync(DateTime dataInicio, DateTime dataFinal);
    Task<IList<MovimentacaoDeProduto>> RelatorioAsync(DateTime dataInicial, DateTime dataFinal, IList<Guid> produtosIds, IList<Guid> pesosIds, IList<Guid> tamanhosIds);
}

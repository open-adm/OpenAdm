using OpenAdm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OpenAdm.Domain.Interfaces;
using OpenAdm.Data.Context;
using OpenAdm.Domain.Model;

namespace OpenAdm.Infra.Repositories;

public class MovimentacaoDeProdutoRepository : GenericRepository<MovimentacaoDeProduto>, IMovimentacaoDeProdutoRepository
{
    public MovimentacaoDeProdutoRepository(ParceiroContext parceiroContext) : base(parceiroContext)
    {
    }

    public async Task AddRangeAsync(IList<MovimentacaoDeProduto> movimentacaoDeProdutos)
    {
        await ParceiroContext.AddRangeAsync(movimentacaoDeProdutos);
    }

    public Task<List<MovimentacaoMensalCategoriaModel>> ObterSaidasPorMesECategoriaAsync(
        DateTime dataInicio, DateTime dataFinal)
    {
        return ParceiroContext.MovimentacoesDeProdutos
            .Where(x => x.DataDeCriacao >= dataInicio && x.DataDeCriacao <= dataFinal &&
                        x.TipoMovimentacaoDeProduto == Domain.Enuns.TipoMovimentacaoDeProduto.Saida)
            .GroupBy(x => new
            {
                Ano = x.DataDeCriacao.Year,
                Mes = x.DataDeCriacao.Month,
                Categoria = x.Produto.Categoria.Descricao
            })
            .Select(g => new MovimentacaoMensalCategoriaModel(
                g.Key.Ano,
                g.Key.Mes,
                g.Key.Categoria,
                g.Sum(x => Math.Truncate(x.QuantidadeMovimentada))))
            .ToListAsync();
    }

    public async Task<IList<MovimentacaoDeProduto>> RelatorioAsync(
        DateTime dataInicial,
        DateTime dataFinal,
        IList<Guid> produtosIds,
        IList<Guid> pesosIds,
        IList<Guid> tamanhosIds)
    {
        var query = ParceiroContext
            .MovimentacoesDeProdutos
            .OrderBy(x => x.DataDeCriacao)
            .Where(x => x.DataDeCriacao.Date >= dataInicial.Date && x.DataDeCriacao.Date <= dataFinal.Date);

        if (produtosIds.Count > 0)
        {
            query = query.Where(x => produtosIds.Contains(x.ProdutoId));
        }

        if (tamanhosIds.Count > 0)
        {
            query = query.Where(x => tamanhosIds.Contains(x.TamanhoId!.Value));
        }

        if (pesosIds.Count > 0)
        {
            query = query.Where(x => pesosIds.Contains(x.PesoId!.Value));
        }

        return await query.ToListAsync();
    }

}

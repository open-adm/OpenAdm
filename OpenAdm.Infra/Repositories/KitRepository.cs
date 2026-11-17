using Microsoft.EntityFrameworkCore;
using OpenAdm.Data.Context;
using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Interfaces;

namespace OpenAdm.Infra.Repositories;

public sealed class KitRepository : GenericRepository<Kit>, IKitRepository
{
    private readonly ParceiroContext _context;

    public KitRepository(ParceiroContext context) : base(context)
    {
        _context = context;
    }

    public Task<Kit?> ObterAsync(Guid id, bool rastrear = false)
    {
        var query = _context.Kits.AsQueryable();
        if (!rastrear)
        {
            query = query.AsNoTracking();
        }

        return query
            .Include(x => x.Itens)
                .ThenInclude(x => x.Produto)
            .Include(x => x.Itens).ThenInclude(x => x.Tamanho)
            .Include(x => x.Itens).ThenInclude(x => x.Peso)
            .Include(x => x.Precos).ThenInclude(x => x.TabelaPreco)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<Kit?> ObterPorNomeAsync(string nome, Guid? ignorarId = null)
    {
        var nomeNormalizado = nome.Trim().ToLower();
        return _context.Kits
            .AsNoTracking()
            .FirstOrDefaultAsync(kit =>
                kit.Nome.ToLower() == nomeNormalizado &&
                (!ignorarId.HasValue || kit.Id != ignorarId.Value));
    }

    public Task<bool> ProdutosExistemAsync(IEnumerable<Guid> produtoIds) =>
        ReferenciasExistemAsync(_context.Produtos.Select(produto => produto.Id), produtoIds);

    public Task<bool> TamanhosExistemAsync(IEnumerable<Guid> tamanhoIds) =>
        ReferenciasExistemAsync(_context.Tamanhos.Select(tamanho => tamanho.Id), tamanhoIds);

    public Task<bool> PesosExistemAsync(IEnumerable<Guid> pesoIds) =>
        ReferenciasExistemAsync(_context.Pesos.Select(peso => peso.Id), pesoIds);

    public Task<bool> TabelasDePrecoExistemAsync(IEnumerable<Guid> tabelaPrecoIds) =>
        ReferenciasExistemAsync(_context.TabelaDePreco.Select(tabela => tabela.Id), tabelaPrecoIds);

    public async Task AddAsync(KitItem item) => 
        await _context.KitsItens.AddAsync(item);

    public async Task AddAsync(KitTabelaPreco preco) => 
        await _context.KitsTabelasPreco.AddAsync(preco);

    private static async Task<bool> ReferenciasExistemAsync(
        IQueryable<Guid> idsExistentes,
        IEnumerable<Guid> ids)
    {
        var idsDistintos = ids.Distinct().ToArray();
        if (idsDistintos.Length == 0)
            return true;

        var quantidadeEncontrada = await idsExistentes.CountAsync(id => idsDistintos.Contains(id));

        return quantidadeEncontrada == idsDistintos.Length;
    }
}

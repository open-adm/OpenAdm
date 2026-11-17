using OpenAdm.Domain.Entities;

namespace OpenAdm.Domain.Interfaces;

public interface IKitRepository : IGenericRepository<Kit>
{
    Task<Kit?> ObterAsync(Guid id, bool rastrear = false);
    Task<Kit?> ObterPorNomeAsync(string nome, Guid? ignorarId = null);
    Task<bool> ProdutosExistemAsync(IEnumerable<Guid> produtoIds);
    Task<bool> TamanhosExistemAsync(IEnumerable<Guid> tamanhoIds);
    Task<bool> PesosExistemAsync(IEnumerable<Guid> pesoIds);
    Task<bool> TabelasDePrecoExistemAsync(IEnumerable<Guid> tabelaPrecoIds);
    Task AddAsync(KitItem item);
    Task AddAsync(KitTabelaPreco preco);
}

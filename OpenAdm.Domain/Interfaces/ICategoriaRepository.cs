using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Model;

namespace OpenAdm.Domain.Interfaces;

public interface ICategoriaRepository : IGenericRepository<Categoria>
{
    Task<IList<Categoria>> GetCategoriasAsync();
    Task<IList<Categoria>> GetCategoriasDropDownAsync();
    Task<Categoria?> GetCategoriaAsync(Guid id);
    Task<Categoria?> GetCategoriaParaAtualizarOrdemAsync(Guid id);
    Task<IList<OrdemExibicaoModel>> GetCategoriasParaOrdemExibicaoAsync();
}

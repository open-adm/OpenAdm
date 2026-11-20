using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Model;
using OpenAdm.Domain.PaginateDto;

namespace OpenAdm.Domain.Interfaces;

public interface ICategoriaRepository : IGenericRepository<Categoria>
{
    Task<IList<Categoria>> GetCategoriasAsync();
    Task<IList<DropDownItemModel>> BuscarDropDownAsync(DropDownFiltro filtro);
    Task<Categoria?> GetCategoriaAsync(Guid id);
    Task<Categoria?> GetCategoriaParaAtualizarOrdemAsync(Guid id);
    Task<IList<OrdemExibicaoModel>> GetCategoriasParaOrdemExibicaoAsync();
    Task<int> GetMaxOrdemExibicaoAsync();
}

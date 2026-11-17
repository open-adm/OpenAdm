using OpenAdm.Application.Dtos.Kits;
using OpenAdm.Application.Models.Kits;
using OpenAdm.Application.Models;
using OpenAdm.Domain.Model;
using OpenAdm.Domain.PaginateDto;
using OpenAdm.Domain.Entities;

namespace OpenAdm.Application.Interfaces;

public interface IKitService
{
    Task<ResultPartner<KitViewModel>> CriarAsync(KitDto dto);
    Task<ResultPartner<KitViewModel>> AtualizarAsync(Guid id, KitDto dto);
    Task<ResultPartner<KitViewModel>> ObterAsync(Guid id);
    Task<ResultPartner<PaginacaoViewModel<KitViewModel>>> PaginarAsync(FilterModel<Kit> filtro);
    Task<ResultPartner<ResultadoPadraoViewModel>> InativarAtivarAsync(Guid id, bool ativo);
}

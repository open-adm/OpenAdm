using OpenAdm.Application.Dtos.AvisosUsuarios;
using OpenAdm.Application.Models;
using OpenAdm.Application.Models.AvisosUsuarios;
using OpenAdm.Domain.Model;

namespace OpenAdm.Application.Interfaces;

public interface IAvisoUsuarioService
{
    Task<ResultPartner<AvisoUsuarioViewModel>> CriarAsync(CriarAvisoUsuarioDto dto);
    Task<ResultPartner<List<AvisoUsuarioViewModel>>> ListarMeusNaoLidosAsync();
    Task<ResultPartner<List<AvisoUsuarioViewModel>>> ListarPorUsuarioAsync(Guid usuarioId);
    Task<ResultPartner<ResultadoPadraoViewModel>> MarcarComoVistoAsync(Guid avisoId);
    Task<ResultPartner<ResultadoPadraoViewModel>> AlterarAtivoAsync(Guid avisoId, bool ativo);
}

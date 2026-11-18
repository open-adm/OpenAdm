using OpenAdm.Domain.Entities;

namespace OpenAdm.Domain.Interfaces;

public interface IAvisoUsuarioRepository
{
    Task<bool> UsuarioExisteAsync(Guid usuarioId);
    Task AddAsync(AvisoUsuario aviso);
    Task<List<AvisoUsuario>> ListarPorUsuarioAsync(Guid usuarioId);
    Task<List<AvisoUsuario>> ListarNaoLidosAsync(Guid usuarioId);
    Task<bool> MarcarComoVistoAsync(Guid usuarioId, Guid avisoId);
    Task<bool> AlterarAtivoAsync(Guid avisoId, bool ativo);
}

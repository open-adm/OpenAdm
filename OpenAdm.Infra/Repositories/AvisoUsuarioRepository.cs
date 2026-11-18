using Microsoft.EntityFrameworkCore;
using OpenAdm.Data.Context;
using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Interfaces;

namespace OpenAdm.Infra.Repositories;

public sealed class AvisoUsuarioRepository(ParceiroContext parceiroContext) : IAvisoUsuarioRepository
{
    public Task<bool> UsuarioExisteAsync(Guid usuarioId) =>
        parceiroContext.Usuarios.AnyAsync(x => x.Id == usuarioId);

    public async Task AddAsync(AvisoUsuario aviso)
    {
        parceiroContext.AvisosUsuarios.Add(aviso);
        await parceiroContext.SaveChangesAsync();
    }

    public Task<List<AvisoUsuario>> ListarPorUsuarioAsync(Guid usuarioId) =>
        OrdenarAvisos(parceiroContext.AvisosUsuarios.Where(x => x.UsuarioId == usuarioId)).ToListAsync();

    public Task<List<AvisoUsuario>> ListarNaoLidosAsync(Guid usuarioId) =>
        OrdenarAvisos(parceiroContext.AvisosUsuarios
                .Where(x => x.UsuarioId == usuarioId && x.Ativo && x.DataDeVisualizacao == null))
            .ToListAsync();

    private static IQueryable<AvisoUsuario> OrdenarAvisos(IQueryable<AvisoUsuario> avisos) =>
        avisos.AsNoTracking()
            .OrderByDescending(x => x.DataDeCriacao)
            .ThenByDescending(x => x.Id);

    public async Task<bool> MarcarComoVistoAsync(Guid usuarioId, Guid avisoId)
    {
        var aviso = parceiroContext.AvisosUsuarios
            .Where(x => x.UsuarioId == usuarioId && x.Id == avisoId && x.Ativo);
        var agora = DateTime.UtcNow;
        var atualizados = await aviso
            .Where(x => x.DataDeVisualizacao == null)
            .ExecuteUpdateAsync(update => update.SetProperty(x => x.DataDeVisualizacao, agora));

        return atualizados > 0 || await aviso.AnyAsync(x => x.DataDeVisualizacao != null);
    }

    public async Task<bool> AlterarAtivoAsync(Guid avisoId, bool ativo)
    {
        var atualizados = await parceiroContext.AvisosUsuarios
            .Where(x => x.Id == avisoId && x.DataDeVisualizacao == null)
            .ExecuteUpdateAsync(update => update.SetProperty(x => x.Ativo, ativo));

        return atualizados > 0;
    }
}

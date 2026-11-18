using OpenAdm.Application.Dtos.AvisosUsuarios;
using OpenAdm.Application.Interfaces;
using OpenAdm.Application.Models;
using OpenAdm.Application.Models.AvisosUsuarios;
using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Enuns;
using OpenAdm.Domain.Interfaces;
using OpenAdm.Domain.Model;

namespace OpenAdm.Application.Services;

public sealed class AvisoUsuarioService(
    IAvisoUsuarioRepository avisoUsuarioRepository,
    IUsuarioAutenticado usuarioAutenticado) : IAvisoUsuarioService
{
    private const string AcessoClienteObrigatorio = "Acesse com uma conta de cliente para consultar avisos.";

    public async Task<ResultPartner<AvisoUsuarioViewModel>> CriarAsync(CriarAvisoUsuarioDto dto)
    {
        var erro = dto.Validar();
        if (erro != null)
            return (ResultPartner<AvisoUsuarioViewModel>)erro;

        if (!await avisoUsuarioRepository.UsuarioExisteAsync(dto.UsuarioId))
            return (ResultPartner<AvisoUsuarioViewModel>)"Usuário destinatário não encontrado.";

        var aviso = AvisoUsuario.Novo(dto.UsuarioId, dto.Mensagem.Trim());
        await avisoUsuarioRepository.AddAsync(aviso);
        return (ResultPartner<AvisoUsuarioViewModel>)AvisoUsuarioViewModel.FromEntity(aviso);
    }

    public async Task<ResultPartner<List<AvisoUsuarioViewModel>>> ListarMeusNaoLidosAsync()
    {
        if (usuarioAutenticado.TipoUsuario != TipoUsuario.Usuario)
            return (ResultPartner<List<AvisoUsuarioViewModel>>)AcessoClienteObrigatorio;

        var avisos = await avisoUsuarioRepository.ListarNaoLidosAsync(usuarioAutenticado.Id);
        return (ResultPartner<List<AvisoUsuarioViewModel>>)avisos.Select(AvisoUsuarioViewModel.FromEntity).ToList();
    }

    public async Task<ResultPartner<List<AvisoUsuarioViewModel>>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        var avisos = await avisoUsuarioRepository.ListarPorUsuarioAsync(usuarioId);
        return (ResultPartner<List<AvisoUsuarioViewModel>>)avisos.Select(AvisoUsuarioViewModel.FromEntity).ToList();
    }

    public async Task<ResultPartner<ResultadoPadraoViewModel>> MarcarComoVistoAsync(Guid avisoId)
    {
        if (usuarioAutenticado.TipoUsuario != TipoUsuario.Usuario)
            return (ResultPartner<ResultadoPadraoViewModel>)AcessoClienteObrigatorio;

        if (!await avisoUsuarioRepository.MarcarComoVistoAsync(usuarioAutenticado.Id, avisoId))
            return (ResultPartner<ResultadoPadraoViewModel>)"Aviso não encontrado.";

        return (ResultPartner<ResultadoPadraoViewModel>)new ResultadoPadraoViewModel { Resultado = true };
    }

    public async Task<ResultPartner<ResultadoPadraoViewModel>> AlterarAtivoAsync(Guid avisoId, bool ativo)
    {
        if (!await avisoUsuarioRepository.AlterarAtivoAsync(avisoId, ativo))
            return (ResultPartner<ResultadoPadraoViewModel>)"Aviso não encontrado ou já visualizado.";

        return (ResultPartner<ResultadoPadraoViewModel>)new ResultadoPadraoViewModel { Resultado = true };
    }
}

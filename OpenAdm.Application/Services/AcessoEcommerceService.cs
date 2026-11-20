using OpenAdm.Application.Interfaces;
using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Interfaces;

namespace OpenAdm.Application.Services;

public class AcessoEcommerceService : IAcessoEcommerceService
{
    private readonly IAcessoEcommerceRepository _acessoEcommerceRepository;
    private readonly IUsuarioAutenticado _usuarioAutenticado;
    public AcessoEcommerceService(IAcessoEcommerceRepository acessoEcommerceRepository, IUsuarioAutenticado usuarioAutenticado)
    {
        _acessoEcommerceRepository = acessoEcommerceRepository;
        _usuarioAutenticado = usuarioAutenticado;
    }

    public async Task AtualizarAcessoAsync()
    {
        var agora = DateTime.UtcNow;
        var acesso = await _acessoEcommerceRepository.ObterPorDataAsync(agora.Year, agora.Month, _usuarioAutenticado.ParceiroId);

        if (acesso == null)
        {
            acesso = AcessoEcommerce.NovoAcesso(1, _usuarioAutenticado.ParceiroId);
            await _acessoEcommerceRepository.AddAsync(acesso);
            return;
        }

        acesso.AdicionarAcesso();
        await _acessoEcommerceRepository.EditAsync(acesso);
    }

    public async Task<long> QuantidadeDeAcessoAsync()
    {
        var agora = DateTime.UtcNow;
        return await _acessoEcommerceRepository.ObterQuantidadePorDataAsync(agora.Year, agora.Month, _usuarioAutenticado.ParceiroId);
    }
}

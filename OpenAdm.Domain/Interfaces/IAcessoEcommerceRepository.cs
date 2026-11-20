using OpenAdm.Domain.Entities;

namespace OpenAdm.Domain.Interfaces;

public interface IAcessoEcommerceRepository
{
    Task AddAsync(AcessoEcommerce acessoEcommerce);
    Task EditAsync(AcessoEcommerce acessoEcommerce);
    Task<long> ObterQuantidadePorDataAsync(int ano, int mes, Guid parceiroId);
    Task<AcessoEcommerce?> ObterPorDataAsync(int ano, int mes, Guid parceiroId);
}

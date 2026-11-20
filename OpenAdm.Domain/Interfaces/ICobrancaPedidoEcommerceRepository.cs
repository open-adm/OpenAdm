using OpenAdm.Domain.Entities.OpenAdm;
using OpenAdm.Domain.Enuns;
using OpenAdm.Domain.Model.Pedidos;

namespace OpenAdm.Domain.Interfaces;

public interface ICobrancaPedidoEcommerceRepository
{
    Task AddAsync(CobrancaPedidoEcommerce cobranca);
    Task SaveChangesAsync();
    Task<CobrancaPedidoEcommerce?> GetByPedidoIdAsync(Guid pedidoId);
    Task AtualizarStatusAsync(Guid id, StatusCobrancaPedidoEcommerceEnum status);
    Task<(decimal Hoje, decimal Semana, decimal Total, int Quantidade)> ObterTotaisAsync(
        DateTime inicioHoje, DateTime inicioSemana);
    Task<ICollection<ItemCobrancaHomeAdmModel>> CobrancasMaisAntigasAsync();
}

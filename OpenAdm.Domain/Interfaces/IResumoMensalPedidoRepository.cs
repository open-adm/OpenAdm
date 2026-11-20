using OpenAdm.Domain.Model.Pedidos;

namespace OpenAdm.Domain.Interfaces;

public interface IResumoMensalPedidoRepository
{
    Task<(ResumoMensalPedidoModel Atual, ResumoMensalPedidoModel Anterior)> ObterComparativoAsync(
        DateTime inicioAtual, DateTime fimAtual, DateTime inicioAnterior, DateTime fimAnterior, Guid? representanteId);
}

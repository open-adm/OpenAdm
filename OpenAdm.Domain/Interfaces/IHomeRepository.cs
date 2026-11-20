using OpenAdm.Domain.Model;
using OpenAdm.Domain.Model.Pedidos;

namespace OpenAdm.Domain.Interfaces;

public interface IHomeRepository
{
    Task<IList<StatusPedidoHomeModel>> ObterStatusPedidosAsync();
    Task<(int Cpf, int Cnpj)> ObterTotalUsuariosPorDocumentoAsync();
    Task<IList<ContadorPedidoModel>> ContatorPedido7DiasAsync(DateTime dataInicio, TimeZoneInfo fusoHorario);
    Task<TotalizadorProtudoEstoqueHome?>  ObterTotalizadoProtudoEstoqueAsync();
    Task<ICollection<ProdutoMaisVendidoModel>> ProdutosMaisVendidosAsync(bool asc);
    Task<TotalParcelasPorVencimentoHomeModel> ObterTotalParcelasPorVencimentoAsync(DateTime hoje);
}

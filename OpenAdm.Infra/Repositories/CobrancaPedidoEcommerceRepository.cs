using Microsoft.EntityFrameworkCore;
using OpenAdm.Data.Context;
using OpenAdm.Domain.Entities.OpenAdm;
using OpenAdm.Domain.Enuns;
using OpenAdm.Domain.Interfaces;
using OpenAdm.Domain.Model.Pedidos;

namespace OpenAdm.Infra.Repositories;

public class CobrancaPedidoEcommerceRepository(ParceiroContext parceiroContext) : ICobrancaPedidoEcommerceRepository
{
    public async Task AddAsync(CobrancaPedidoEcommerce cobranca)
    {
        await parceiroContext.CobrancasPedidosEcommerce.AddAsync(cobranca);
    }

    public async Task SaveChangesAsync() => await parceiroContext.SaveChangesAsync();

    public async Task<CobrancaPedidoEcommerce?> GetByPedidoIdAsync(Guid pedidoId)
    {
        return await parceiroContext
            .CobrancasPedidosEcommerce
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PedidoId == pedidoId);
    }

    public async Task AtualizarStatusAsync(Guid id, StatusCobrancaPedidoEcommerceEnum status)
    {
        await parceiroContext
            .CobrancasPedidosEcommerce
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(y => y.Status, status)
                .SetProperty(y => y.DataDeAtualizacao, DateTime.UtcNow));
    }

    public async Task<(decimal Hoje, decimal Semana, decimal Total, int Quantidade)> ObterTotaisAsync(
        DateTime inicioHoje, DateTime inicioSemana)
    {
        var totais = await CobrancasPendentes()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Hoje = g.Sum(x => x.DataDeCriacao >= inicioHoje ? x.Total : 0),
                Semana = g.Sum(x => x.DataDeCriacao >= inicioSemana ? x.Total : 0),
                Total = g.Sum(x => x.Total),
                Quantidade = g.Count()
            })
            .FirstOrDefaultAsync();

        return (totais?.Hoje ?? 0, totais?.Semana ?? 0, totais?.Total ?? 0, totais?.Quantidade ?? 0);
    }

    public async Task<ICollection<ItemCobrancaHomeAdmModel>> CobrancasMaisAntigasAsync()
    {
        return await CobrancasPendentes()
            .OrderBy(x => x.DataDeCriacao)
            .ThenBy(x => x.Id)
            .Take(3)
            .Select(x => new ItemCobrancaHomeAdmModel
            {
                PedidoId = x.PedidoId,
                NumeroPedido = x.Pedido.Numero,
                Cliente = x.Pedido.Usuario.Nome,
                Valor = x.Total,
                Data = x.DataDeCriacao
            })
            .ToListAsync();
    }

    private IQueryable<CobrancaPedidoEcommerce> CobrancasPendentes()
    {
        return parceiroContext.CobrancasPedidosEcommerce
            .Where(x => x.Status == StatusCobrancaPedidoEcommerceEnum.ACobrar && x.Ativo);
    }
}

using Microsoft.EntityFrameworkCore;
using OpenAdm.Data.Context;
using OpenAdm.Domain.Enuns;
using OpenAdm.Domain.Interfaces;
using OpenAdm.Domain.Model.Pedidos;

namespace OpenAdm.Infra.Repositories;

public sealed class ResumoMensalPedidoRepository : IResumoMensalPedidoRepository
{
    private readonly ParceiroContext _context;

    public ResumoMensalPedidoRepository(ParceiroContext context)
    {
        _context = context;
    }

    public async Task<(ResumoMensalPedidoModel Atual, ResumoMensalPedidoModel Anterior)> ObterComparativoAsync(
        DateTime inicioAtual, DateTime fimAtual, DateTime inicioAnterior, DateTime fimAnterior, Guid? representanteId)
    {
        var pedidos = _context.Pedidos
            .Where(x => x.StatusPedido == StatusPedido.Entregue &&
                        (x.DataDeCriacao >= inicioAtual && x.DataDeCriacao < fimAtual ||
                         x.DataDeCriacao >= inicioAnterior && x.DataDeCriacao < fimAnterior));

        if (representanteId.HasValue)
        {
            pedidos = pedidos.Where(x => x.RepresentanteId == representanteId.Value);
        }

        var valoresPorPedido =
            from pedido in pedidos
            join item in _context.ItensPedidos on pedido.Id equals item.PedidoId into itens
            from item in itens.DefaultIfEmpty()
            group item by new { pedido.Id, Atual = pedido.DataDeCriacao >= inicioAtual } into g
            select new
            {
                g.Key.Atual,
                Quantidade = g.Sum(x => x == null ? 0 : x.Quantidade),
                Valor = g.Sum(x => x == null ? 0 : x.ValorUnitario * x.Quantidade)
            };

        var totais = await valoresPorPedido
            .GroupBy(x => x.Atual)
            .Select(g => new
            {
                Atual = g.Key,
                Pedidos = g.Count(),
                Valor = g.Sum(x => x.Valor),
                Itens = g.Sum(x => x.Quantidade)
            })
            .ToListAsync();

        var categorias = await (
            from pedido in pedidos
            join item in _context.ItensPedidos on pedido.Id equals item.PedidoId
            group item by new
            {
                Atual = pedido.DataDeCriacao >= inicioAtual,
                item.Produto.CategoriaId,
                item.Produto.Categoria.Descricao
            } into g
            select new
            {
                g.Key.Atual,
                Categoria = new ResumoMensalCategoriaModel
                {
                    CategoriaId = g.Key.CategoriaId,
                    Categoria = g.Key.Descricao,
                    QuantidadeItensVendidos = g.Sum(x => x.Quantidade)
                }
            }).ToListAsync();

        ResumoMensalPedidoModel MontarResumo(bool periodoAtual)
        {
            var total = totais.SingleOrDefault(x => x.Atual == periodoAtual);
            return new ResumoMensalPedidoModel
            {
                QuantidadePedidos = total?.Pedidos ?? 0,
                ValorTotalVendido = total?.Valor ?? 0,
                QuantidadeItensVendidos = total?.Itens ?? 0,
                Categorias = categorias.Where(x => x.Atual == periodoAtual).Select(x => x.Categoria).ToList()
            };
        }

        return (MontarResumo(true), MontarResumo(false));
    }
}

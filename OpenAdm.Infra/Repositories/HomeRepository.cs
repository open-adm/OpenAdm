using Microsoft.EntityFrameworkCore;
using OpenAdm.Data.Context;
using OpenAdm.Domain.Enuns;
using OpenAdm.Domain.Interfaces;
using OpenAdm.Domain.Model;
using OpenAdm.Domain.Model.Pedidos;

namespace OpenAdm.Infra.Repositories;

public class HomeRepository : IHomeRepository
{
    private readonly ParceiroContext _parceiroContext;

    public HomeRepository(ParceiroContext parceiroContext)
    {
        _parceiroContext = parceiroContext;
    }

    public async Task<IList<StatusPedidoHomeModel>> ObterStatusPedidosAsync()
    {
        return await _parceiroContext
            .Pedidos
            .AsNoTracking()
            .GroupBy(x => x.StatusPedido)
            .Select(x => new StatusPedidoHomeModel()
            {
                Status = x.Key,
                Quantidade = x.Count()
            })
            .OrderByDescending(y => y.Quantidade)
            .ToListAsync();
    }

    public async Task<TotalParcelasPorVencimentoHomeModel> ObterTotalParcelasPorVencimentoAsync(DateTime hoje)
    {
        var amanha = hoje.AddDays(1);
        var fimDaSemana = hoje.AddDays(8);

        return await _parceiroContext
            .Parcelas
            .AsNoTracking()
            .Where(x => !x.Quitada &&
                        x.DataDeVencimento >= hoje &&
                        x.DataDeVencimento < fimDaSemana &&
                        (x.Tipo == TipoFaturaEnum.AReceber || x.Tipo == TipoFaturaEnum.APagar))
            .GroupBy(_ => 1)
            .Select(parcelas => new TotalParcelasPorVencimentoHomeModel
            {
                AReceberHoje = parcelas.Sum(x =>
                    x.Tipo == TipoFaturaEnum.AReceber && x.DataDeVencimento < amanha ? x.Valor : 0),
                AReceberSemana = parcelas.Sum(x =>
                    x.Tipo == TipoFaturaEnum.AReceber && x.DataDeVencimento >= amanha ? x.Valor : 0),
                APagarHoje = parcelas.Sum(x =>
                    x.Tipo == TipoFaturaEnum.APagar && x.DataDeVencimento < amanha ? x.Valor : 0),
                APagarSemana = parcelas.Sum(x =>
                    x.Tipo == TipoFaturaEnum.APagar && x.DataDeVencimento >= amanha ? x.Valor : 0)
            })
            .FirstOrDefaultAsync() ?? new TotalParcelasPorVencimentoHomeModel();
    }

    public async Task<(int Cpf, int Cnpj)> ObterTotalUsuariosPorDocumentoAsync()
    {
        var totais = await _parceiroContext.Usuarios
            .Where(x => x.Ativo)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Cpf = g.Count(x => !string.IsNullOrWhiteSpace(x.Cpf)),
                Cnpj = g.Count(x => !string.IsNullOrWhiteSpace(x.Cnpj))
            })
            .FirstOrDefaultAsync();

        return (totais?.Cpf ?? 0, totais?.Cnpj ?? 0);
    }

    public async Task<ICollection<ProdutoMaisVendidoModel>> ProdutosMaisVendidosAsync(bool asc)
    {
        var totais = _parceiroContext.ItensPedidos
            .Where(x => x.Pedido.StatusPedido == StatusPedido.Entregue)
            .GroupBy(x => new { x.ProdutoId, x.PesoId, x.TamanhoId })
            .Select(g => new
            {
                g.Key.ProdutoId,
                g.Key.PesoId,
                g.Key.TamanhoId,
                Quantidade = g.Sum(x => x.Quantidade),
                ValorUnitario = g.Max(x => x.ValorUnitario)
            });

        var ordenados = asc ? totais.OrderBy(x => x.Quantidade) : totais.OrderByDescending(x => x.Quantidade);
        var primeiros = ordenados
            .ThenBy(x => x.ProdutoId)
            .ThenBy(x => x.PesoId)
            .ThenBy(x => x.TamanhoId)
            .Take(3);

        var produtos = await (
            from item in primeiros
            join produto in _parceiroContext.Produtos on item.ProdutoId equals produto.Id
            join peso in _parceiroContext.Pesos on item.PesoId equals (Guid?)peso.Id into pesos
            from peso in pesos.DefaultIfEmpty()
            join tamanho in _parceiroContext.Tamanhos on item.TamanhoId equals (Guid?)tamanho.Id into tamanhos
            from tamanho in tamanhos.DefaultIfEmpty()
            select new ProdutoMaisVendidoModel
            {
                Id = item.ProdutoId,
                Descricao = produto.Descricao,
                Foto = produto.UrlFoto,
                Quantidade = item.Quantidade,
                ValorUnitario = item.ValorUnitario,
                Peso = peso == null ? null : peso.Descricao,
                Tamanho = tamanho == null ? null : tamanho.Descricao
            }).ToListAsync();

        return asc
            ? produtos.OrderBy(x => x.Quantidade).ToList()
            : produtos.OrderByDescending(x => x.Quantidade).ToList();
    }

    public async Task<ICollection<ProdutoMaisVendidoModel>> ProdutosMenosVendidosAsync()
    {
        return await _parceiroContext.ItensPedidos
            .AsNoTracking()
            .Where(x => !x.Pedido.Excluido)
            .GroupBy(x => new
            {
                x.ProdutoId,
                x.PesoId,
                x.TamanhoId,
                x.Produto.Descricao,
                x.Produto.UrlFoto,
                PesoDescricao = x.Peso!.Descricao,
                TamanhoDescricao = x.Tamanho!.Descricao
            })
            .Select(g => new ProdutoMaisVendidoModel
            {
                Id = g.Key.ProdutoId,
                Descricao = g.Key.Descricao,
                Foto = g.Key.UrlFoto,
                Quantidade = g.Sum(x => x.Quantidade),
                Peso = g.Key.PesoDescricao,
                Tamanho = g.Key.TamanhoDescricao
            })
            .OrderBy(x => x.Quantidade)
            .Take(3)
            .ToListAsync();
    }

    public async Task<TotalizadorProtudoEstoqueHome?> ObterTotalizadoProtudoEstoqueAsync()
    {
        var quantidade = await _parceiroContext
            .Estoques
            .AsNoTracking()
            .SumAsync(x => (decimal?)x.Quantidade) ?? 0;

        var quantidadeReservada = await _parceiroContext
            .ItensPedidos
            .AsNoTracking()
            .Where(x => x.Pedido.StatusPedido == StatusPedido.Aberto && !x.Pedido.Excluido)
            .SumAsync(x => (decimal?)x.Quantidade) ?? 0;

        return new TotalizadorProtudoEstoqueHome
        {
            Quantidade = quantidade,
            QuantidadeReservada = quantidadeReservada
        };
    }

    public async Task<IList<ContadorPedidoModel>> ContatorPedido7DiasAsync(DateTime dataInicio, TimeZoneInfo fusoHorario)
    {
        var limites = Enumerable.Range(0, 8)
            .Select(dia => TimeZoneInfo.ConvertTimeToUtc(dataInicio.AddDays(dia), fusoHorario))
            .ToArray();

        var totais = await _parceiroContext.Pedidos
            .Where(x => x.DataDeCriacao >= limites[0] && x.DataDeCriacao < limites[7])
            .GroupBy(x =>
                x.DataDeCriacao < limites[1] ? 0 :
                x.DataDeCriacao < limites[2] ? 1 :
                x.DataDeCriacao < limites[3] ? 2 :
                x.DataDeCriacao < limites[4] ? 3 :
                x.DataDeCriacao < limites[5] ? 4 :
                x.DataDeCriacao < limites[6] ? 5 : 6)
            .Select(g => new { Dia = g.Key, Total = g.Count() })
            .ToListAsync();

        return totais.Select(x => new ContadorPedidoModel
        {
            Data = dataInicio.AddDays(x.Dia),
            Total = x.Total
        }).ToList();
    }
}

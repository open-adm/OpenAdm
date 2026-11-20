using OpenAdm.Application.Interfaces;
using OpenAdm.Application.Models.Home;
using OpenAdm.Domain.Interfaces;
using OpenAdm.Domain.Model.Pedidos;

namespace OpenAdm.Application.Services;

public class HomeSevice : IHomeSevice
{
    private static readonly TimeZoneInfo FusoBrasilia = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    private readonly IMovimentacaoDeProdutosService _movimentacaoDeProdutosService;
    private readonly IAcessoEcommerceService _acessoEcommerceService;
    private readonly IHomeRepository _homeRepository;
    private readonly IUsuarioAutenticado _usuarioAutenticado;
    private readonly ICachedService<HomeAdmViewModel> _cache;
    private readonly ICobrancaPedidoEcommerceRepository _cobrancaPedidoEcommerceRepository;
    private readonly IResumoMensalHomeService _resumoMensalHomeService;

    public HomeSevice(
        IMovimentacaoDeProdutosService movimentacaoDeProdutosService,
        IAcessoEcommerceService acessoEcommerceService,
        IHomeRepository homeRepository, IUsuarioAutenticado usuarioAutenticado, ICachedService<HomeAdmViewModel> cache,
        ICobrancaPedidoEcommerceRepository cobrancaPedidoEcommerceRepository,
        IResumoMensalHomeService resumoMensalHomeService)
    {
        _movimentacaoDeProdutosService = movimentacaoDeProdutosService;
        _acessoEcommerceService = acessoEcommerceService;
        _homeRepository = homeRepository;
        _usuarioAutenticado = usuarioAutenticado;
        _cache = cache;
        _cobrancaPedidoEcommerceRepository = cobrancaPedidoEcommerceRepository;
        _resumoMensalHomeService = resumoMensalHomeService;
    }

    public async Task<HomeAdmViewModel> GetHomeAdmAsync()
    {
        var agoraUtc = DateTime.UtcNow;
        var hoje = TimeZoneInfo.ConvertTimeFromUtc(agoraUtc, FusoBrasilia).Date;
        var key = $"home_{_usuarioAutenticado.ParceiroId}_{hoje:yyyyMMdd}";

        var cache = await _cache.GetItemAsync(key);

        // if (cache != null)
        // {
        //     return cache;
        // }

        var movimentos = await _movimentacaoDeProdutosService.MovimentoDashBoardAsync();
        var pedidosStatus = await _homeRepository.ObterStatusPedidosAsync();
        var totalPedidos = pedidosStatus.Sum(x => x.Quantidade);
        foreach (var status in pedidosStatus)
        {
            status.Porcentagem = Math.Round((decimal)status.Quantidade * 100 / totalPedidos, 2);
        }

        var quantidadeDeAcessoEcommerce = await _acessoEcommerceService.QuantidadeDeAcessoAsync();
        var (quantidadeDeUsuarioCpf, quantidadeDeUsuarioCnpj) = await _homeRepository.ObterTotalUsuariosPorDocumentoAsync();
        var totalizadorProdutoEstoque = await _homeRepository.ObterTotalizadoProtudoEstoqueAsync();
        var dataInicio = hoje.AddDays(-6);
        var pedidosPorDia = await _homeRepository.ContatorPedido7DiasAsync(dataInicio, FusoBrasilia);
        var produtosMaisVendidos = await _homeRepository.ProdutosMaisVendidosAsync(false);
        var produtosMenosVendidos = await _homeRepository.ProdutosMaisVendidosAsync(true);
        var totaisParcelas = await _homeRepository
            .ObterTotalParcelasPorVencimentoAsync(agoraUtc.Date);
        var resumoMensal = await _resumoMensalHomeService.ObterAsync();

        var inicioHojeUtc = TimeZoneInfo.ConvertTimeToUtc(hoje, FusoBrasilia);
        var totaisCobranca = await _cobrancaPedidoEcommerceRepository.ObterTotaisAsync(
            inicioHojeUtc, agoraUtc.AddDays(-7));
        var cobrancasMaisAntigas = await _cobrancaPedidoEcommerceRepository.CobrancasMaisAntigasAsync();

        cache = new HomeAdmViewModel()
        {
            ResumoMensal = resumoMensal,
            Parcelas = new()
            {
                AReceberHoje = totaisParcelas.AReceberHoje,
                AReceberSemana = totaisParcelas.AReceberSemana,
                APagarHoje = totaisParcelas.APagarHoje,
                APagarSemana = totaisParcelas.APagarSemana
            },
            Cobranca = new()
            {
                TotalHoje = totaisCobranca.Hoje,
                TotalSemana = totaisCobranca.Semana,
                QuantidadeACobrar = totaisCobranca.Quantidade,
                TotalCobranca = totaisCobranca.Total,
                CobrancasMaisAntigas = cobrancasMaisAntigas.Select(x => new ItemCobrancaHomeAdmViewModel
                {
                    PedidoId = x.PedidoId,
                    Cliente = x.Cliente,
                    NumeroPedido = x.NumeroPedido,
                    Valor = x.Valor,
                    Data = x.Data
                }).ToList()
            },
            PedidosPorDia = MontarCountDiario(pedidosPorDia, dataInicio),
            Movimentos = movimentos,
            TotalDePedidos = totalPedidos,
            ProdutosMaisVendidos = produtosMaisVendidos,
            ProdutosMenosVendidos = produtosMenosVendidos,
            TotalProdutoEstoque = totalizadorProdutoEstoque?.Quantidade ?? 0,
            TotalProdutoEstoqueReservado = totalizadorProdutoEstoque?.QuantidadeReservada ?? 0,
            StatusPedido = pedidosStatus,
            QuantidadeDeAcessoEcommerce = quantidadeDeAcessoEcommerce,
            QuantidadeDeUsuarioCnpj = quantidadeDeUsuarioCnpj,
            QuantidadeDeUsuarioCpf = quantidadeDeUsuarioCpf,
        };

        await _cache.SetItemAsync(key, cache);

        return cache;
    }

    public async Task<HomeRepresentanteViewModel> GetHomeRepresentanteAsync(Guid representanteId) => new()
    {
        ResumoMensal = await _resumoMensalHomeService.ObterAsync(representanteId)
    };

    private static IList<PedidoPorDiaModel> MontarCountDiario(IList<ContadorPedidoModel> registros, DateTime dataInicio)
    {
        var porData = registros.ToDictionary(r => r.Data.Date, r => r.Total);
        var resultado = new List<PedidoPorDiaModel>(7);

        for (var i = 0; i < 7; i++)
        {
            var data = dataInicio.AddDays(i);
            resultado.Add(new PedidoPorDiaModel
            {
                Data = data,
                Total = porData.TryGetValue(data, out var total) ? total : 0
            });
        }

        return resultado;
    }
}

using OpenAdm.Application.Interfaces;
using OpenAdm.Application.Models.MovimentacaoDeProdutos;
using OpenAdm.Domain.Interfaces;
using OpenAdm.Domain.Model;
using OpenAdm.Domain.Extensions;
using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Enuns;
using OpenAdm.Application.Dtos.MovimentosDeProdutos;
using System.Text;
using OpenAdm.Domain.PaginateDto;
using OpenAdm.Pdf.Interfaces;
using OpenAdm.Pdf.DTOs;

namespace OpenAdm.Application.Services;

public class MovimentacaoDeProdutosService : IMovimentacaoDeProdutosService
{
    private readonly IMovimentacaoDeProdutoRepository _movimentacaoDeProdutorepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IPesoRepository _pesoRepository;
    private readonly ITamanhoRepository _tamanhoRepository;
    private readonly IMovimentacaoDeProdutoRelatorioService _movimentacaoDeProdutoRelatorioService;
    private readonly IParceiroAutenticado _parceiroAutenticado;

    public MovimentacaoDeProdutosService(
        IMovimentacaoDeProdutoRepository movimentacaoDeProdutorepository,
        IProdutoRepository produtoRepository,
        IPesoRepository pesoRepository,
        ITamanhoRepository tamanhoRepository,
        IMovimentacaoDeProdutoRelatorioService movimentacaoDeProdutoRelatorioService,
        IParceiroAutenticado parceiroAutenticado)
    {
        _movimentacaoDeProdutorepository = movimentacaoDeProdutorepository;
        _produtoRepository = produtoRepository;
        _pesoRepository = pesoRepository;
        _tamanhoRepository = tamanhoRepository;
        _movimentacaoDeProdutoRelatorioService = movimentacaoDeProdutoRelatorioService;
        _parceiroAutenticado = parceiroAutenticado;
    }

    public async Task<byte[]> GerarRelatorioAsync(RelatorioMovimentoDeProdutoDto relatorioMovimentoDeProdutoDto)
    {
        var parceiro = await _parceiroAutenticado.ObterParceiroAutenticadoAsync();
        var logo = parceiro.Logo is null ? null : Encoding.UTF8.GetString(parceiro.Logo);
        var movimentacoes = await _movimentacaoDeProdutorepository.RelatorioAsync(
            dataInicial: relatorioMovimentoDeProdutoDto.DataInicial,
            dataFinal: relatorioMovimentoDeProdutoDto.DataFinal,
            produtosIds: relatorioMovimentoDeProdutoDto.ProdutosId,
            pesosIds: relatorioMovimentoDeProdutoDto.PesosId,
            tamanhosIds: relatorioMovimentoDeProdutoDto.TamanhosId);

        var produtosids = movimentacoes
            .DistinctBy(x => x.ProdutoId)
            .Select(x => x.ProdutoId)
            .ToList();

        var tamanhosIds = movimentacoes
            .Where(x => x.TamanhoId != null)
            .DistinctBy(x => x.TamanhoId)
            .Select(x => x.TamanhoId!.Value)
            .ToList();

        var pesosIds = movimentacoes
            .Where(x => x.PesoId != null)
            .DistinctBy(x => x.PesoId)
            .Select(x => x.PesoId!.Value)
            .ToList();

        var produtos = await _produtoRepository.GetDictionaryProdutosAsync(produtosids);
        var tamanhos = await _tamanhoRepository.GetDescricaoTamanhosAsync(tamanhosIds);
        var pesos = await _pesoRepository.GetDescricaoPesosAsync(pesosIds);

        var movimentacoesRelatorio = new List<MovimentacaoDeProdutoRelatorioDTO>();
        IList<RelatorioMovimentoDeProdutoTotalizacaoDTO> totalCategorias = [];
        IList<RelatorioMovimentoDeProdutoTotalizacaoDTO> totalPesos = [];
        IList<RelatorioMovimentoDeProdutoTotalizacaoDTO> totalTamanhos = [];

        foreach (var item in movimentacoes)
        {
            if (produtos.TryGetValue(item.ProdutoId, out var produto))
            {
                var totalCategoria = totalCategorias.FirstOrDefault(x => x.Descricao == produto.Categoria.Descricao);

                if (totalCategoria == null)
                {
                    totalCategorias.Add(new RelatorioMovimentoDeProdutoTotalizacaoDTO()
                    {
                        Descricao = produto.Categoria.Descricao,
                        Total = item.QuantidadeMovimentada
                    });
                }
                else
                {
                    totalCategoria.Total += item.QuantidadeMovimentada;
                }

                var pesoTamanho = string.Empty;

                if (item.TamanhoId.HasValue && tamanhos.TryGetValue(item.TamanhoId.Value, out var tamanho))
                {
                    pesoTamanho = tamanho;
                }

                if (item.PesoId.HasValue && pesos.TryGetValue(item.PesoId.Value, out var peso))
                {
                    pesoTamanho = peso;
                }

                movimentacoesRelatorio.Add(new()
                {
                    Categoria = produto.Categoria.Descricao,
                    DataDaMovimentacao = item.DataDeCriacao,
                    Descricao = produto.Descricao,
                    Quantidade = item.QuantidadeMovimentada,
                    Referencia = produto.Referencia,
                    TipoMovimento = item.TipoMovimentacaoDeProduto.ToString(),
                    PesoTamanho = pesoTamanho
                });
            }
        }

        var groupedByPesos = movimentacoes
            .Where(x => x.PesoId != null)
            .GroupBy(p => p.PesoId)
            .Select(group => new
            {
                Peso = group.Key!.Value
            });

        var groupedByTamanhos = movimentacoes
            .Where(x => x.TamanhoId != null)
            .GroupBy(p => p.TamanhoId)
            .Select(group => new
            {
                Tamanho = group.Key!.Value
            });

        foreach (var peso in groupedByPesos)
        {
            if (pesos.TryGetValue(peso.Peso, out var pes))
            {
                totalPesos.Add(new RelatorioMovimentoDeProdutoTotalizacaoDTO()
                {
                    Descricao = pes,
                    Total = movimentacoes.Where(x => x.PesoId == peso.Peso).Sum(x => x.QuantidadeMovimentada)
                });
            }
        }

        foreach (var tamanho in groupedByTamanhos)
        {
            if (tamanhos.TryGetValue(tamanho.Tamanho, out var tama))
            {
                totalTamanhos.Add(new RelatorioMovimentoDeProdutoTotalizacaoDTO()
                {
                    Descricao = tama,
                    Total = movimentacoes.Where(x => x.TamanhoId == tamanho.Tamanho).Sum(x => x.QuantidadeMovimentada)
                });
            }
        }

        return _movimentacaoDeProdutoRelatorioService.ObterPdfAsync(
            movimentacoesRelatorio,
            parceiro.NomeFantasia,
            relatorioMovimentoDeProdutoDto.DataInicial,
            relatorioMovimentoDeProdutoDto.DataFinal,
            logo,
            totalCategoria: totalCategorias,
            totalPesos: totalPesos,
            totalTamanhos: totalTamanhos);
    }

    public Task<PaginacaoViewModel<MovimentacaoDeProdutoViewModel>>
        GetPaginacaoAsync(FilterModel<MovimentacaoDeProduto> paginacaoMovimentacaoDeProdutoDto)
    {
        return _movimentacaoDeProdutorepository.PaginacaoProjetadaAsync(
            paginacaoMovimentacaoDeProdutoDto,
            x => new MovimentacaoDeProdutoViewModel
            {
                Id = x.Id,
                DataDeCriacao = x.DataDeCriacao,
                DataDeAtualizacao = x.DataDeAtualizacao,
                Numero = x.Numero,
                QuantidadeMovimentada = x.QuantidadeMovimentada,
                TipoMovimentacaoDeProduto = x.TipoMovimentacaoDeProduto,
                Produto = x.Produto.Descricao,
                Peso = x.Peso!.Descricao,
                Tamanho = x.Tamanho!.Descricao
            });
    }

    public async Task MovimentarItensPedidoAsync(IList<ItemPedido> itens)
    {
        var data = DateTime.UtcNow;
        var movimentos = itens.Select(x => new MovimentacaoDeProduto(
            id: Guid.NewGuid(),
            dataDeCriacao: data,
            dataDeAtualizacao: data,
            numero: 0,
            quantidadeMovimentada: x.Quantidade,
            tipoMovimentacaoDeProduto: TipoMovimentacaoDeProduto.Saida,
            produtoId: x.ProdutoId,
            tamanhoId: x.TamanhoId,
            pesoId: x.PesoId,
            observacao: null
        )).ToList();
        await _movimentacaoDeProdutorepository.AddRangeAsync(movimentos);
    }

    public async Task<IList<MovimentoDeProdutoDashBoardModel>> MovimentoDashBoardAsync()
    {
        var agora = DateTime.UtcNow;
        var inicio = new DateTime(agora.Year, agora.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-3);
        var movimentos = await _movimentacaoDeProdutorepository.ObterSaidasPorMesECategoriaAsync(inicio, agora);

        return movimentos
            .GroupBy(x => new { x.Ano, x.Mes })
            .Select(g => new MovimentoDeProdutoDashBoardModel
            {
                Data = new DateTime(g.Key.Ano, g.Key.Mes, 1),
                Mes = g.Key.Mes.ConverterMesIntEmNome(),
                Dados = g.Select(x => new DadosMovimentoDeProdutoDashBoardModel
                {
                    Categoria = x.Categoria,
                    Quantidade = (int)x.Quantidade
                }).ToList()
            })
            .OrderBy(x => x.Data)
            .ToList();
    }
}

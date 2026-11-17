using OpenAdm.Domain.Entities;
using OpenAdm.Application.Models;

namespace OpenAdm.Application.Models.Kits;

public sealed class KitViewModel : BaseModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Imagem { get; set; }
    public bool Ativo { get; set; }
    public bool VenderPessoaFisica { get; set; }
    public bool VenderPessoaJuridica { get; set; }
    public List<KitItemViewModel> Itens { get; set; } = [];
    public List<KitPrecoViewModel> Precos { get; set; } = [];

    public static explicit operator KitViewModel(Kit kit) => new()
    {
        Id = kit.Id,
        DataDeCriacao = kit.DataDeCriacao,
        DataDeAtualizacao = kit.DataDeAtualizacao,
        Numero = kit.Numero,
        Nome = kit.Nome,
        Descricao = kit.Descricao,
        Imagem = kit.Imagem,
        Ativo = kit.Ativo,
        VenderPessoaFisica = kit.VenderPessoaFisica,
        VenderPessoaJuridica = kit.VenderPessoaJuridica,
        Itens = kit.Itens
            .Where(item => item.Ativo)
            .Select(item => (KitItemViewModel)item)
            .ToList(),
        Precos = kit.Precos
            .Where(preco => preco.Ativo)
            .Select(preco => (KitPrecoViewModel)preco)
            .ToList()
    };
}

public sealed class KitItemViewModel
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public string Produto { get; set; } = string.Empty;
    public Guid? TamanhoId { get; set; }
    public string? Tamanho { get; set; }
    public Guid? PesoId { get; set; }
    public string? Peso { get; set; }
    public int Quantidade { get; set; }

    public static explicit operator KitItemViewModel(KitItem item) => new()
    {
        Id = item.Id,
        ProdutoId = item.ProdutoId,
        Produto = item.Produto?.Descricao ?? string.Empty,
        TamanhoId = item.TamanhoId,
        Tamanho = item.Tamanho?.Descricao,
        PesoId = item.PesoId,
        Peso = item.Peso?.Descricao,
        Quantidade = item.Quantidade
    };
}

public sealed class KitPrecoViewModel
{
    public Guid TabelaPrecoId { get; set; }
    public string TabelaPreco { get; set; } = string.Empty;
    public decimal? PrecoPessoaFisica { get; set; }
    public decimal? PrecoPessoaJuridica { get; set; }

    public static explicit operator KitPrecoViewModel(KitTabelaPreco preco) => new()
    {
        TabelaPrecoId = preco.TabelaPrecoId,
        TabelaPreco = preco.TabelaPreco?.Descricao ?? string.Empty,
        PrecoPessoaFisica = preco.PrecoPessoaFisica,
        PrecoPessoaJuridica = preco.PrecoPessoaJuridica
    };
}

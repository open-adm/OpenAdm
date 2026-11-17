using OpenAdm.Application.Attributes;
using OpenAdm.Domain.Constants;

namespace OpenAdm.Application.Dtos.Kits;

public sealed class KitDto : ValidarBaseDTO
{
    [ValidaString(maxLength: KitMaxLength.Nome)]
    public string Nome { get; set; } = string.Empty;
    [ValidaStringLength(maxLength: KitMaxLength.Descricao)]
    public string? Descricao { get; set; }
    public string? NovaFoto { get; set; }
    public bool VenderPessoaFisica { get; set; }
    public bool VenderPessoaJuridica { get; set; }
    public List<KitItemDto> Itens { get; set; } = [];
    public List<KitPrecoDto> Precos { get; set; } = [];

    public override string? Validar()
    {
        var erro = base.Validar();
        if (erro != null)
        {
            return erro;
        }

        if (!VenderPessoaFisica && !VenderPessoaJuridica)
        {
            return "O kit deve estar disponível para pessoa física ou jurídica";
        }

        if (Itens.Count == 0)
        {
            return "Informe ao menos um componente para o kit";
        }

        if (Itens.Any(item => item.Quantidade <= 0))
        {
            return "A quantidade dos componentes deve ser maior que zero";
        }

        if (Itens.GroupBy(item => (item.ProdutoId, item.TamanhoId, item.PesoId))
            .Any(grupo => grupo.Count() > 1))
        {
            return "Não é permitido duplicar componentes do kit";
        }

        if (Precos.GroupBy(preco => preco.TabelaPrecoId).Any(grupo => grupo.Count() > 1))
        {
            return "Não é permitido duplicar tabelas de preço";
        }

        return null;
    }
}

public sealed class KitItemDto
{
    public Guid ProdutoId { get; set; }
    public Guid? TamanhoId { get; set; }
    public Guid? PesoId { get; set; }
    public int Quantidade { get; set; }
}

public sealed class KitPrecoDto
{
    public Guid TabelaPrecoId { get; set; }
    public decimal? PrecoPessoaFisica { get; set; }
    public decimal? PrecoPessoaJuridica { get; set; }
}


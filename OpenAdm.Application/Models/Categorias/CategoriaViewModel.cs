using OpenAdm.Domain.Entities;
using OpenAdm.Application.Models.Produtos;

namespace OpenAdm.Application.Models.Categorias;

public class CategoriaViewModel : BaseModel
{
    public string Descricao { get; set; } = string.Empty;
    public string? Foto { get; set; }
    public bool InativoEcommerce { get; set; }
    public bool Ativo { get; set; }
    public int OrdemExibixao { get; set; } = 0;
    public List<ProdutoViewModel>? Produtos { get; set; }

    public CategoriaViewModel ToModel(Categoria entity)
    {
        Id = entity.Id;
        DataDeCriacao = entity.DataDeCriacao;
        DataDeAtualizacao = entity.DataDeAtualizacao;
        Numero = entity.Numero;
        Descricao = entity.Descricao;
        Foto = entity.Foto;
        Produtos = entity.Produtos.Select(x => new ProdutoViewModel().ToModel(x)).ToList();
        InativoEcommerce = entity.InativoEcommerce;
        Ativo = !entity.InativoEcommerce;
        OrdemExibixao = entity.OrdemExibixao;

        return this;
    }

    public CategoriaViewModel ToViewModel(Categoria entity)
    {
        Ativo = entity.InativoEcommerce;
        Id = entity.Id;
        DataDeCriacao = entity.DataDeCriacao;
        DataDeAtualizacao = entity.DataDeAtualizacao;
        Numero = entity.Numero;
        Descricao = entity.Descricao;
        Foto = entity.Foto;
        InativoEcommerce = !entity.InativoEcommerce;
        OrdemExibixao = entity.OrdemExibixao;

        return this;
    }
}

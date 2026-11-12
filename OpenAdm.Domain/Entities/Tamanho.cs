using OpenAdm.Domain.Entities.Bases;

namespace OpenAdm.Domain.Entities;

public sealed class Tamanho : BaseEntity
{
    public Tamanho(
        Guid id,
        DateTime dataDeCriacao,
        DateTime dataDeAtualizacao,
        long numero,
        string descricao,
        decimal? pesoReal, decimal? alturaReal, decimal? larguraReal, decimal? comprimentoReal, bool ativo,
        int ordemExibixao)
        : base(id, dataDeCriacao, dataDeAtualizacao, numero)
    {
        Descricao = descricao;
        PesoReal = pesoReal;
        AlturaReal = alturaReal;
        LarguraReal = larguraReal;
        ComprimentoReal = comprimentoReal;
        Ativo = ativo;
        OrdemExibixao = ordemExibixao;
    }

    public void Update(string descricao, decimal? pesoReal, decimal? alturaReal, decimal? larguraReal,
        decimal? comprimentoReal, int ordemExibixao)
    {
        Descricao = descricao;
        PesoReal = pesoReal;
        AlturaReal = alturaReal;
        LarguraReal = larguraReal;
        ComprimentoReal = comprimentoReal;
        OrdemExibixao = ordemExibixao;
    }

    public decimal? PesoReal { get; set; }
    public decimal? AlturaReal { get; set; }
    public decimal? LarguraReal { get; set; }
    public decimal? ComprimentoReal { get; set; }
    public string Descricao { get; private set; }
    public bool Ativo { get; private set; }
    public int OrdemExibixao { get; private set; }
    public List<ItemPedido> ItensPedido { get; set; } = [];
    public List<Produto> Produtos { get; set; } = [];

    public void InativarAtivar(bool ativo)
    {
        Ativo = ativo;
    }
}

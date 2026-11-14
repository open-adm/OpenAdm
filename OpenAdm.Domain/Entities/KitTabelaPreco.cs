namespace OpenAdm.Domain.Entities;

public class KitTabelaPreco
{
    public KitTabelaPreco(Guid id, bool ativo, Guid kitId, Guid tabelaPrecoId, decimal? precoPessoaFisica,
        decimal? precoPessoaJuridica)
    {
        Id = id;
        Ativo = ativo;
        KitId = kitId;
        TabelaPrecoId = tabelaPrecoId;
        PrecoPessoaFisica = precoPessoaFisica;
        PrecoPessoaJuridica = precoPessoaJuridica;
    }

    public Guid Id { get; private set; }
    public bool Ativo { get; private set; }
    public Guid KitId { get; private set; }
    public Kit Kit { get; set; } = null!;
    public Guid TabelaPrecoId { get; private set; }
    public TabelaDePreco TabelaPreco { get; set; } = null!;
    public decimal? PrecoPessoaFisica { get; private set; }
    public decimal? PrecoPessoaJuridica { get; private set; }

    public void Atualizar(bool ativo, Guid tabelaPrecoId, decimal? precoPessoaFisica, decimal? precoPessoaJuridica)
    {
        Ativo = ativo;
        TabelaPrecoId = tabelaPrecoId;
        PrecoPessoaFisica = precoPessoaFisica;
        PrecoPessoaJuridica = precoPessoaJuridica;
    }

    public static KitTabelaPreco Novo(Guid kitId, Guid tabelaPrecoId, decimal? precoPessoaFisica,
        decimal? precoPessoaJuridica)
    {
        return new KitTabelaPreco(Guid.NewGuid(), true, kitId, tabelaPrecoId, precoPessoaFisica,
            precoPessoaJuridica);
    }
}

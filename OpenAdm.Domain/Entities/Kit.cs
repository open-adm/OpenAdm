using OpenAdm.Domain.Entities.Bases;

namespace OpenAdm.Domain.Entities;

public sealed class Kit : BaseEntity
{
    public Kit(
        Guid id,
        DateTime dataDeCriacao,
        DateTime dataDeAtualizacao,
        long numero, string nome, string? descricao, string? imagem, bool ativo, bool venderPessoaFisica, bool venderPessoaJuridica)
        : base(id, dataDeCriacao, dataDeAtualizacao, numero)
    {
        Nome = nome;
        Descricao = descricao;
        Imagem = imagem;
        Ativo = ativo;
        VenderPessoaFisica = venderPessoaFisica;
        VenderPessoaJuridica = venderPessoaJuridica;
    }
    
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }
    public string? Imagem { get; private set; }
    public bool Ativo { get; private set; }
    public bool VenderPessoaFisica { get; private set; }
    public bool VenderPessoaJuridica { get; private set; }

    public ICollection<KitItem> Itens { get; set; } = [];
    public ICollection<KitTabelaPreco> Precos { get; set; } = [];

    public void Atualizar(string nome, string? descricao, string? imagem, bool ativo, bool venderPessoaFisica,
        bool venderPessoaJuridica)
    {
        Nome = nome;
        Descricao = descricao;
        Imagem = imagem;
        Ativo = ativo;
        VenderPessoaFisica = venderPessoaFisica;
        VenderPessoaJuridica = venderPessoaJuridica;
        DataDeAtualizacao = DateTime.UtcNow;
    }

    public static Kit Novo(string nome, string? descricao, string? imagem, bool venderPessoaFisica,
        bool venderPessoaJuridica)
    {
        var agora = DateTime.UtcNow;
        return new Kit(Guid.NewGuid(), agora, agora, 0, nome, descricao, imagem, true, venderPessoaFisica,
            venderPessoaJuridica);
    }
}

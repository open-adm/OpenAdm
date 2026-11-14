namespace OpenAdm.Domain.Entities;

public class KitItem
{
    public KitItem(Guid id, bool ativo, Guid kitId, Guid produtoId, Guid? tamanhoId, Guid? pesoId, int quantidade)
    {
        Id = id;
        Ativo = ativo;
        KitId = kitId;
        ProdutoId = produtoId;
        TamanhoId = tamanhoId;
        PesoId = pesoId;
        Quantidade = quantidade;
    }

    public Guid Id { get; private set; }
    public bool Ativo { get; private set; }
    public Guid KitId { get; private set; }
    public Kit Kit { get; set; } = null!;
    public Guid ProdutoId { get; private set; }
    public Produto Produto { get; set; } = null!;
    public Guid? TamanhoId { get; private set; }
    public Tamanho? Tamanho { get; set; } = null!;
    public Guid? PesoId { get; private set; }
    public Peso? Peso { get; set; } = null!;
    public int Quantidade { get; private set; }

    public void Atualizar(bool ativo, Guid produtoId, Guid? tamanhoId, Guid? pesoId, int quantidade)
    {
        Ativo = ativo;
        ProdutoId = produtoId;
        TamanhoId = tamanhoId;
        PesoId = pesoId;
        Quantidade = quantidade;
    }

    public static KitItem Novo(Guid kitId, Guid produtoId, Guid? tamanhoId, Guid? pesoId, int quantidade)
    {
        return new KitItem(Guid.NewGuid(), true, kitId, produtoId, tamanhoId, pesoId, quantidade);
    }
}

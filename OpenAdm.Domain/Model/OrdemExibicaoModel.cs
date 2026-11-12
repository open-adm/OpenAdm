namespace OpenAdm.Domain.Model;

public sealed class OrdemExibicaoModel
{
    public Guid Id { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public string? Foto { get; init; }
    public int OrdemExibicao { get; set; }
}

namespace OpenAdm.Domain.Entities;

public sealed class AvisoUsuario
{
    public const int MensagemMaxLength = 2000;

    public AvisoUsuario(
        Guid id,
        Guid usuarioId,
        string mensagem,
        DateTime dataDeCriacao,
        DateTime? dataDeVisualizacao,
        bool ativo)
    {
        Id = id;
        UsuarioId = usuarioId;
        Mensagem = mensagem;
        DataDeCriacao = dataDeCriacao;
        DataDeVisualizacao = dataDeVisualizacao;
        Ativo = ativo;
    }

    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string Mensagem { get; private set; }
    public DateTime DataDeCriacao { get; private set; }
    public DateTime? DataDeVisualizacao { get; private set; }
    public bool Ativo { get; private set; }

    public static AvisoUsuario Novo(Guid usuarioId, string mensagem) =>
        new(Guid.NewGuid(), usuarioId, mensagem, DateTime.UtcNow, dataDeVisualizacao: null, ativo: true);
}

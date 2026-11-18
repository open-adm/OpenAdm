using OpenAdm.Domain.Entities;

namespace OpenAdm.Application.Models.AvisosUsuarios;

public sealed record AvisoUsuarioViewModel(
    Guid Id,
    Guid UsuarioId,
    string Mensagem,
    DateTime DataDeCriacao,
    DateTime? DataDeVisualizacao,
    bool Ativo)
{
    public bool Visto => DataDeVisualizacao.HasValue;

    public static AvisoUsuarioViewModel FromEntity(AvisoUsuario aviso) =>
        new(aviso.Id, aviso.UsuarioId, aviso.Mensagem, aviso.DataDeCriacao, aviso.DataDeVisualizacao, aviso.Ativo);
}

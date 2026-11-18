using OpenAdm.Application.Attributes;
using OpenAdm.Domain.Entities;

namespace OpenAdm.Application.Dtos.AvisosUsuarios;

public sealed class CriarAvisoUsuarioDto : ValidarBaseDTO
{
    [ValidaGuid("Informe o usuário destinatário.")]
    public Guid UsuarioId { get; set; }

    [ValidaString("Informe a mensagem do aviso.", maxLength: AvisoUsuario.MensagemMaxLength)]
    public string Mensagem { get; set; } = string.Empty;
}

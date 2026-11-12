using OpenAdm.Application.Dtos;

namespace OpenAdm.Application.Dtos.OrdemExibicao;

public sealed class AtualizarOrdemExibicaoDto : ValidarBaseDTO
{
    public Guid IdOrigem { get; set; }
    public int PosicaoOrigem { get; set; }
    public Guid IdDestino { get; set; }
    public int PosicaoDestino { get; set; }

    public override string? Validar()
    {
        if (IdOrigem == Guid.Empty)
            return "Informe o id de origem";

        if (IdDestino == Guid.Empty)
            return "Informe o id de destino";

        if (IdOrigem == IdDestino)
            return "Os cadastros de origem e destino devem ser diferentes";

        return base.Validar();
    }
}

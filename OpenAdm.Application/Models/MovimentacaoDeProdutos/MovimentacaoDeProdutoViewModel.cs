using OpenAdm.Domain.Enuns;

namespace OpenAdm.Application.Models.MovimentacaoDeProdutos;

public class MovimentacaoDeProdutoViewModel : BaseModel
{
    public decimal QuantidadeMovimentada { get; set; }
    public TipoMovimentacaoDeProduto TipoMovimentacaoDeProduto { get; set; }
    public string? Produto { get; set; }
    public string? Tamanho { get; set; }
    public string? Peso { get; set; }
}

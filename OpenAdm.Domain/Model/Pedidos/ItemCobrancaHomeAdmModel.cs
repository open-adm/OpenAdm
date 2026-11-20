namespace OpenAdm.Domain.Model.Pedidos;

public class ItemCobrancaHomeAdmModel
{
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public Guid PedidoId { get; set; }
    public long NumeroPedido { get; set; }
    public string Cliente { get; set; } = string.Empty;
}
using OpenAdm.Application.Dtos.Pedidos;
using OpenAdm.Application.Interfaces;
using OpenAdm.Application.Interfaces.Pedidos;
using OpenAdm.Application.Models.Pedidos;
using OpenAdm.Domain.Enuns;
using OpenAdm.Domain.Entities;
using OpenAdm.Domain.Exceptions;
using OpenAdm.Domain.Interfaces;

namespace OpenAdm.Application.Services.Pedidos;

public class UpdateStatusPedidoService : IUpdateStatusPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IMovimentacaoDeProdutosService _movimentacaoDeProdutosService;
    private readonly IEstoqueService _estoqueService;

    public UpdateStatusPedidoService(
        IPedidoRepository pedidoRepository,
        IMovimentacaoDeProdutosService movimentacaoDeProdutosService,
        IEstoqueService estoqueService)
    {
        _pedidoRepository = pedidoRepository;
        _movimentacaoDeProdutosService = movimentacaoDeProdutosService;
        _estoqueService = estoqueService;
    }

    public async Task<PedidoViewModel> UpdateStatusPedidoAsync(UpdateStatusPedidoDto updateStatusPedidoDto)
    {
        var pedido = await _pedidoRepository.ObterPedidoParaAtualizarStatusAsync(updateStatusPedidoDto.PedidoId)
                     ?? throw new ExceptionApi("Não foi possível localizar o pedido");
        var statusAnterior = pedido.StatusPedido;
        pedido.UpdateStatus(updateStatusPedidoDto.StatusPedido);
        _pedidoRepository.Update(pedido);

        if (pedido.StatusPedido == StatusPedido.Entregue)
        {
            await _movimentacaoDeProdutosService.MovimentarItensPedidoAsync(pedido.ItensPedido);
            await _estoqueService.MovimentacaoDePedidoEntregueAsync(pedido.ItensPedido);

            await _pedidoRepository.AddHistoricoAsync(PedidoHistorico.Novo(
                pedido.Id,
                "Pedido entregue"));
        }
        else
        {
            if (statusAnterior != pedido.StatusPedido)
            {
                await _pedidoRepository.AddHistoricoAsync(PedidoHistorico.Novo(
                    pedido.Id,
                    $"Status do pedido alterado de {statusAnterior} para {pedido.StatusPedido}."));
            }
        }

        await _pedidoRepository.SaveChangesAsync();

        return new PedidoViewModel().ForModel(pedido);
    }
}

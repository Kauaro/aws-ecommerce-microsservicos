using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces;


    public interface IPedidoRepository
    {
        Task<Pedido> CriarAsync(Pedido pedido);
        Task<Pedido?> BuscarPorIdAsync(string pedidoId);
    }

using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Interfaces;

public interface IPagamentoRepository
{
    Task<Pagamento?> BuscarPorPedidoIdAsync(string pedidoId);
    Task SalvarAsync(Pagamento pagamento);
}
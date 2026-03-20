using PaymentService.Domain.Entities;
using PaymentService.Domain.Events;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.SQS;

namespace PaymentService.Application.UseCases;

public class ProcessarPagamentoUseCase
{
    private readonly IPagamentoRepository _repository;
    private readonly SqsPublisher _publisher;
    private readonly string _paymentProcessedQueueUrl;

    public ProcessarPagamentoUseCase(
        IPagamentoRepository repository,
        SqsPublisher publisher,
        IConfiguration configuration)
    {
        _repository = repository;
        _publisher = publisher;
        _paymentProcessedQueueUrl = configuration["AWS:PaymentProcessedQueueUrl"]!;
    }

    public async Task<Pagamento?> ExecutarAsync(PedidoCriadoEvent evento)
    {
        // Idempotência — verifica se já foi processado
        var existente = await _repository.BuscarPorPedidoIdAsync(evento.PedidoId);
        if (existente != null)
        {
            Console.WriteLine($"[IDEMPOTÊNCIA] Pedido {evento.PedidoId} já foi processado.");
            return null;
        }

        // Simula processamento do pagamento
        var status = evento.ValorTotal > 0 ? "APROVADO" : "RECUSADO";

        var pagamento = new Pagamento
        {
            PedidoId = evento.PedidoId,
            Valor = evento.ValorTotal,
            Status = status
        };

        await _repository.SalvarAsync(pagamento);

        // Publica evento para o NotificationService
        var pagamentoProcessadoEvent = new PagamentoProcessadoEvent
        {
            PedidoId = evento.PedidoId,
            ClienteNome = evento.ClienteNome,
            ClienteEmail = evento.ClienteEmail,
            Valor = evento.ValorTotal,
            Status = status
        };

        await _publisher.PublicarAsync(pagamentoProcessadoEvent, _paymentProcessedQueueUrl);
        Console.WriteLine($"[PAGAMENTO] Pedido {evento.PedidoId} — {status} — R$ {evento.ValorTotal}");

        return pagamento;
    }
}
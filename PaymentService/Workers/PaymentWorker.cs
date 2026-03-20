using PaymentService.Application.UseCases;
using PaymentService.Domain.Events;
using PaymentService.Infrastructure.SQS;

namespace PaymentService.Workers;

public class PaymentWorker : BackgroundService
{
    private readonly SqsConsumer _consumer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentWorker> _logger;

    public PaymentWorker(
        SqsConsumer consumer,
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentWorker> logger)
    {
        _consumer = consumer;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[WORKER] PaymentWorker iniciado — aguardando mensagens...");

        while (!stoppingToken.IsCancellationRequested)
        {
            var (evento, receiptHandle) = await _consumer.ReceberAsync<PedidoCriadoEvent>();

            if (evento == null)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            _logger.LogInformation($"[WORKER] Mensagem recebida: Pedido {evento.PedidoId}");

            // Cria um escopo para cada mensagem processada
            using var scope = _scopeFactory.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<ProcessarPagamentoUseCase>();

            await useCase.ExecutarAsync(evento);
            await _consumer.DeletarMensagemAsync(receiptHandle);

            _logger.LogInformation($"[WORKER] Mensagem processada e deletada: {evento.PedidoId}");
        }
    }
}
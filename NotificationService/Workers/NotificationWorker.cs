using NotificationService.Application.UseCases;
using NotificationService.Domain.Events;
using NotificationService.Infrastructure.SQS;

namespace NotificationService.Workers;

public class NotificationWorker : BackgroundService
{
    private readonly SqsConsumer _consumer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationWorker> _logger;



    public NotificationWorker(
        SqsConsumer consumer,
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationWorker> logger)
    {
        _consumer = consumer;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }



    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[WORKER] NotificationWorker iniciado — aguardando mensagens...");

        while (!stoppingToken.IsCancellationRequested)
        {
            var (evento, receiptHandle) = await _consumer.ReceberAsync<PagamentoProcessadoEvent>();

            if (evento == null)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            _logger.LogInformation($"[WORKER] Mensagem recebida: Pedido {evento.PedidoId}");

            using var scope = _scopeFactory.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<EnviarNotificacaoUseCase>();

            await useCase.ExecutarAsync(evento);
            await _consumer.DeletarMensagemAsync(receiptHandle);

            _logger.LogInformation($"[WORKER] Notificação enviada e mensagem deletada: {evento.PedidoId}");
        }
    }
}
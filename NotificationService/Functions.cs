using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NotificationService.Application.UseCases;
using NotificationService.Domain.Events;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NotificationService;

public class Functions
{
    private readonly IServiceProvider _serviceProvider;

    public Functions()
    {
        var startup = new Startup(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build());

        var services = new ServiceCollection();
        startup.ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            context.Logger.LogInformation($"[LAMBDA] Mensagem recebida: {record.Body}");

            var evento = JsonConvert.DeserializeObject<PagamentoProcessadoEvent>(record.Body);
            if (evento == null) continue;

            using var scope = _serviceProvider.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<EnviarNotificacaoUseCase>();

            await useCase.ExecutarAsync(evento);

            context.Logger.LogInformation($"[LAMBDA] Notificação enviada: Pedido {evento.PedidoId}");
        }
    }
}
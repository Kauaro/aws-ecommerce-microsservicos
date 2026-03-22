using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PaymentService.Application.UseCases;
using PaymentService.Domain.Events;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PaymentService;

public class Functions
{
    private readonly IServiceProvider _serviceProvider;

    public Functions()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);

        var startup = new Startup(configuration);
        startup.ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            context.Logger.LogInformation($"[LAMBDA] Mensagem recebida: {record.Body}");

            var evento = JsonConvert.DeserializeObject<PedidoCriadoEvent>(record.Body);
            if (evento == null) continue;

            using var scope = _serviceProvider.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<ProcessarPagamentoUseCase>();

            await useCase.ExecutarAsync(evento);

            context.Logger.LogInformation($"[LAMBDA] Pagamento processado: Pedido {evento.PedidoId}");
        }
    }
}
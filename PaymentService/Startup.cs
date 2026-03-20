using Amazon.DynamoDBv2;
using Amazon.SQS;
using PaymentService.Application.UseCases;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.DynamoDB;
using PaymentService.Infrastructure.SQS;
using PaymentService.Workers;

namespace PaymentService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var serviceUrl = Configuration["AWS:ServiceURL"];

        // DynamoDB
        services.AddSingleton<IAmazonDynamoDB>(_ =>
            new AmazonDynamoDBClient(
                new Amazon.Runtime.BasicAWSCredentials("test", "test"),
                new AmazonDynamoDBConfig { ServiceURL = serviceUrl }
            )
        );

        // SQS
        services.AddSingleton<IAmazonSQS>(_ =>
            new AmazonSQSClient(
                new Amazon.Runtime.BasicAWSCredentials("test", "test"),
                new AmazonSQSConfig { ServiceURL = serviceUrl }
            )
        );

        // Repositórios, Consumer e UseCases
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<ProcessarPagamentoUseCase>();
        services.AddSingleton<SqsConsumer>();
        services.AddSingleton<SqsPublisher>();

        // Worker que fica ouvindo a fila
        services.AddHostedService<PaymentWorker>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
        app.UseRouting();
    }
}
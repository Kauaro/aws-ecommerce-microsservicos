using Amazon.DynamoDBv2;
using Amazon.SQS;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
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

    private static AWSCredentials GetCredentials()
    {
        var chain = new CredentialProfileStoreChain();
        if (chain.TryGetAWSCredentials("aws-dev", out var credentials))
            return credentials;
        return FallbackCredentialsFactory.GetCredentials();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // DynamoDB
        services.AddSingleton<IAmazonDynamoDB>(_ =>
            new AmazonDynamoDBClient(
                GetCredentials(),
                new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                }
            )
        );

        // SQS
        services.AddSingleton<IAmazonSQS>(_ =>
            new AmazonSQSClient(
                GetCredentials(),
                new AmazonSQSConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                }
            )
        );

        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<ProcessarPagamentoUseCase>();
        services.AddSingleton<SqsConsumer>();
        services.AddSingleton<SqsPublisher>();
        services.AddHostedService<PaymentWorker>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
        app.UseRouting();
    }
}
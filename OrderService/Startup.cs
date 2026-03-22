using Amazon.DynamoDBv2;
using Amazon.SQS;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using FluentValidation.AspNetCore;
using OrderService.Application.UseCases;
using OrderService.Domain.Interfaces;
using OrderService.Domain.Events;
using OrderService.Infrastructure.DynamoDB;
using OrderService.Infrastructure.SQS;

namespace OrderService;

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
        services.AddControllers();

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

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IEventPublisher, SqsPublisher>();
        services.AddScoped<CriarPedidoUseCase>();
        services.AddFluentValidationAutoValidation();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
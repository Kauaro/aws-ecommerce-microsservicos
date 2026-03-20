using Amazon.DynamoDBv2;
using Amazon.SQS;
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

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

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

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();


        // Repositórios, Publishers e UseCases
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
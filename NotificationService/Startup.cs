using Amazon.SimpleEmail;
using Amazon.SQS;
using NotificationService.Application.UseCases;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.SES;
using NotificationService.Infrastructure.SQS;
using NotificationService.Workers;

namespace NotificationService;

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

        // SQS
        services.AddSingleton<IAmazonSQS>(_ =>
            new AmazonSQSClient(
                new Amazon.Runtime.BasicAWSCredentials("test", "test"),
                new AmazonSQSConfig { ServiceURL = serviceUrl }
            )
        );

        // SES
        services.AddSingleton<IAmazonSimpleEmailService>(_ =>
            new AmazonSimpleEmailServiceClient(
                new Amazon.Runtime.BasicAWSCredentials("test", "test"),
                new AmazonSimpleEmailServiceConfig { ServiceURL = serviceUrl }
            )
        );

        services.AddSingleton<SqsConsumer>();
        services.AddScoped<IEmailSender, SesEmailSender>();
        services.AddScoped<EnviarNotificacaoUseCase>();
        services.AddHostedService<NotificationWorker>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
        app.UseRouting();
    }
}
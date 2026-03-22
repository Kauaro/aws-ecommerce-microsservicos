using Amazon.SimpleEmail;
using Amazon.SQS;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
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

    private static AWSCredentials GetCredentials()
    {
        var chain = new CredentialProfileStoreChain();
        if (chain.TryGetAWSCredentials("aws-dev", out var credentials))
            return credentials;
        return FallbackCredentialsFactory.GetCredentials();
    }

    public void ConfigureServices(IServiceCollection services)
    {
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

        // SES
        services.AddSingleton<IAmazonSimpleEmailService>(_ =>
            new AmazonSimpleEmailServiceClient(
                GetCredentials(),
                new AmazonSimpleEmailServiceConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                }
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
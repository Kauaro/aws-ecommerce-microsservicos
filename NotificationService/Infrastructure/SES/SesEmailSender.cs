using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.SES;

public class SesEmailSender : IEmailSender
{

    private readonly IAmazonSimpleEmailService _sesClient;
    private readonly string _remetente;


    public SesEmailSender(IAmazonSimpleEmailService sesClient, IConfiguration configuration)
    {
        _sesClient = sesClient;
        _remetente = configuration["AWS:EmailRemetente"]!;
    }


    public async Task EnviarAsync(string destinatario, string assunto, string corpo)
    {


        var request = new SendEmailRequest
        {
            Source = _remetente,
            Destination = new Destination { ToAddresses = new List<string> { destinatario } },
            Message = new Message
            {
                Subject = new Content(assunto),
                Body = new Body
                {
                    Html = new Content(corpo)
                }
            }
        };


        await _sesClient.SendEmailAsync(request);
        Console.WriteLine($"[SES] E-mail enviado para {destinatario}");
    }
}
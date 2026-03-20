namespace NotificationService.Domain.Interfaces;

public interface IEmailSender
{
    Task EnviarAsync(string destinatario, string assunto, string corpo);
}
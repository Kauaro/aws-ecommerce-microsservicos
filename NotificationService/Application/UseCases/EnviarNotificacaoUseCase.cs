using NotificationService.Domain.Events;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.UseCases;

public class EnviarNotificacaoUseCase
{


    private readonly IEmailSender _emailSender;

    public EnviarNotificacaoUseCase(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }



    public async Task ExecutarAsync(PagamentoProcessadoEvent evento)
    {
        var emoji = evento.Status == "APROVADO" ? "✅" : "❌";
        var assunto = $"{emoji} Pedido {evento.PedidoId} — Pagamento {evento.Status}";

        var corpo = $"""
            <h2>Olá, {evento.ClienteNome}!</h2>
            <p>Seu pedido <strong>{evento.PedidoId}</strong> foi processado.</p>
            <p>Status do pagamento: <strong>{evento.Status}</strong></p>
            <p>Valor: <strong>R$ {evento.Valor:F2}</strong></p>
            <p>Data: {evento.ProcessadoEm:dd/MM/yyyy HH:mm}</p>
            <br/>
            <p>Obrigado pela sua compra!</p>
            """;

        await _emailSender.EnviarAsync(evento.ClienteEmail, assunto, corpo);

        Console.WriteLine($"[NOTIFICAÇÃO] E-mail enviado para {evento.ClienteEmail} — Pedido {evento.PedidoId} {evento.Status}");
    }
}
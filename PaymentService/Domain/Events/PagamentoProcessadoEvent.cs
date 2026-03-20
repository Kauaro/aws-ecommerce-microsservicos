namespace PaymentService.Domain.Events;

public class PagamentoProcessadoEvent
{
    public string PedidoId { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ProcessadoEm { get; set; } = DateTime.UtcNow;
}
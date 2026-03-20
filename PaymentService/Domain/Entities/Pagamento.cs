namespace PaymentService.Domain.Entities;

public class Pagamento
{
    public string PagamentoId { get; set; } = Guid.NewGuid().ToString();
    public string PedidoId { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ProcessadoEm { get; set; } = DateTime.UtcNow;
}
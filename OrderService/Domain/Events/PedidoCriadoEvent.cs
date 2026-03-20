namespace OrderService.Domain.Events
{ 
    public class PedidoCriadoEvent
    {
    public string PedidoId { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
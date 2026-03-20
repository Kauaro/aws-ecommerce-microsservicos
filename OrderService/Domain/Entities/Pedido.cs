namespace OrderService.Domain.Entities
{ 
    public class Pedido
    {
        public string PedidoId { get; set; } = Guid.NewGuid().ToString();
        public string ClienteNome { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        public List<ItemPedido> Itens { get; set; } = new();
        public decimal ValorTotal => Itens.Sum(i => i.Preco * i.Quantidade);
        public string Status { get; set; } = "Pendente";
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }


    public class ItemPedido
    {
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
    }
}
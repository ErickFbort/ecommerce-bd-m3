namespace ECommerceM3.Models;

// Compra realizada por um cliente (tabela: pedido). Reune varios itens.
public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime DataPedido { get; set; }
    public string Status { get; set; } = "PENDENTE";
    public decimal ValorTotal { get; set; }

    // Preenchido em consultas com JOIN (exibicao amigavel)
    public string? ClienteNome { get; set; }

    // Itens associados ao pedido (tabela item_pedido)
    public List<ItemPedido> Itens { get; set; } = new();
}

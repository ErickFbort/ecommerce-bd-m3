namespace ECommerceM3.Models;

// Associativa entre pedido e produto (tabela: item_pedido)
public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }

    // Preenchido em consultas com JOIN (exibicao amigavel)
    public string? ProdutoNome { get; set; }

    public decimal Subtotal => Quantidade * PrecoUnitario;
}

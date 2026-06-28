namespace ECommerceM3.Models;

// Item vendido na plataforma (tabela: produto). Pertence a uma categoria.
public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int CategoriaId { get; set; }

    // Preenchido apenas em consultas com JOIN (exibicao amigavel)
    public string? CategoriaNome { get; set; }
}

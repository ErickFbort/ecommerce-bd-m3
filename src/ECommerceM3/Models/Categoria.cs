namespace ECommerceM3.Models;

// Agrupa produtos por tipo (tabela: categoria)
public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}

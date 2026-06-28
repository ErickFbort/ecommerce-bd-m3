namespace ECommerceM3.Models;

// Representa o comprador da plataforma (tabela: cliente)
public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }

    // Telefones do cliente (tabela telefone_cliente) - atributo multivalorado
    public List<TelefoneCliente> Telefones { get; set; } = new();
}

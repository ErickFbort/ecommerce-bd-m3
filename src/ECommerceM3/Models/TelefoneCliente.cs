namespace ECommerceM3.Models;

// Telefone de um cliente (tabela: telefone_cliente).
// Atributo multivalorado extraido para tabela propria na 1FN.
public class TelefoneCliente
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string? Tipo { get; set; }
}

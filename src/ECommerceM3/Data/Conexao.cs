using MySqlConnector;

namespace ECommerceM3.Data;

// Fabrica centralizada de conexoes com o MySQL.
// Ajuste a connection string conforme o seu ambiente (usuario/senha/porta).
public static class Conexao
{
    // ATENCAO: altere "senha" para a senha do seu MySQL local.
    private const string ConnectionString =
        "Server=localhost;Port=3306;Database=ecommerce_m3;User ID=root;Password=senha;";

    public static MySqlConnection Criar()
    {
        var conexao = new MySqlConnection(ConnectionString);
        conexao.Open();
        return conexao;
    }
}

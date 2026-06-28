using MySqlConnector;

namespace ECommerceM3.Data;

// Fabrica centralizada de conexoes com o MySQL.
// Ajuste a connection string conforme o seu ambiente (usuario/senha/porta).
public static class Conexao
{
    // Senha alinhada com a definida no docker-compose.yml (MYSQL_ROOT_PASSWORD).
    // Ajuste se o seu MySQL usar outra senha/porta.
    private const string ConnectionString =
        "Server=localhost;Port=3306;Database=ecommerce_m3;User ID=root;Password=senha123;";

    public static MySqlConnection Criar()
    {
        var conexao = new MySqlConnection(ConnectionString);
        conexao.Open();
        return conexao;
    }
}

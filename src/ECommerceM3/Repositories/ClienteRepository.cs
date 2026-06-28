using ECommerceM3.Data;
using ECommerceM3.Models;
using MySqlConnector;

namespace ECommerceM3.Repositories;

// CRUD da entidade Cliente usando ADO.NET (SQL escrito a mao, parametrizado).
public class ClienteRepository
{
    // CREATE: INSERT INTO cliente (nome, email, cpf, telefone) VALUES (...)
    public int Inserir(Cliente cliente)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"INSERT INTO cliente (nome, email, cpf, telefone)
                             VALUES (@nome, @email, @cpf, @telefone);
                             SELECT LAST_INSERT_ID();";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@nome", cliente.Nome);
        cmd.Parameters.AddWithValue("@email", cliente.Email);
        cmd.Parameters.AddWithValue("@cpf", cliente.Cpf);
        cmd.Parameters.AddWithValue("@telefone", (object?)cliente.Telefone ?? DBNull.Value);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // READ (todos): SELECT * FROM cliente ORDER BY nome
    public List<Cliente> ListarTodos()
    {
        var lista = new List<Cliente>();
        using var conexao = Conexao.Criar();
        const string sql = @"SELECT id, nome, email, cpf, telefone, data_cadastro
                             FROM cliente ORDER BY nome;";
        using var cmd = new MySqlCommand(sql, conexao);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            lista.Add(Mapear(reader));
        return lista;
    }

    // READ (por id): SELECT * FROM cliente WHERE id = @id
    public Cliente? BuscarPorId(int id)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"SELECT id, nome, email, cpf, telefone, data_cadastro
                             FROM cliente WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Mapear(reader) : null;
    }

    // UPDATE: UPDATE cliente SET ... WHERE id = @id
    public bool Atualizar(Cliente cliente)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"UPDATE cliente
                             SET nome = @nome, email = @email, cpf = @cpf, telefone = @telefone
                             WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@nome", cliente.Nome);
        cmd.Parameters.AddWithValue("@email", cliente.Email);
        cmd.Parameters.AddWithValue("@cpf", cliente.Cpf);
        cmd.Parameters.AddWithValue("@telefone", (object?)cliente.Telefone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", cliente.Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    // DELETE: DELETE FROM cliente WHERE id = @id
    public bool Remover(int id)
    {
        using var conexao = Conexao.Criar();
        const string sql = "DELETE FROM cliente WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    private static Cliente Mapear(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"),
        Nome = reader.GetString("nome"),
        Email = reader.GetString("email"),
        Cpf = reader.GetString("cpf"),
        Telefone = reader.IsDBNull(reader.GetOrdinal("telefone")) ? null : reader.GetString("telefone"),
        DataCadastro = reader.GetDateTime("data_cadastro")
    };
}

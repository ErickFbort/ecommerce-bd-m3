using ECommerceM3.Data;
using ECommerceM3.Models;
using MySqlConnector;

namespace ECommerceM3.Repositories;

// CRUD da entidade Categoria usando ADO.NET (SQL parametrizado).
public class CategoriaRepository
{
    // CREATE: INSERT INTO categoria (nome, descricao) VALUES (...)
    public int Inserir(Categoria categoria)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"INSERT INTO categoria (nome, descricao)
                             VALUES (@nome, @descricao);
                             SELECT LAST_INSERT_ID();";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@nome", categoria.Nome);
        cmd.Parameters.AddWithValue("@descricao", (object?)categoria.Descricao ?? DBNull.Value);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // READ (todos): SELECT * FROM categoria ORDER BY nome
    public List<Categoria> ListarTodos()
    {
        var lista = new List<Categoria>();
        using var conexao = Conexao.Criar();
        const string sql = "SELECT id, nome, descricao FROM categoria ORDER BY nome;";
        using var cmd = new MySqlCommand(sql, conexao);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            lista.Add(Mapear(reader));
        return lista;
    }

    // READ (por id): SELECT * FROM categoria WHERE id = @id
    public Categoria? BuscarPorId(int id)
    {
        using var conexao = Conexao.Criar();
        const string sql = "SELECT id, nome, descricao FROM categoria WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Mapear(reader) : null;
    }

    // UPDATE: UPDATE categoria SET ... WHERE id = @id
    public bool Atualizar(Categoria categoria)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"UPDATE categoria
                             SET nome = @nome, descricao = @descricao
                             WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@nome", categoria.Nome);
        cmd.Parameters.AddWithValue("@descricao", (object?)categoria.Descricao ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", categoria.Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    // DELETE: DELETE FROM categoria WHERE id = @id
    public bool Remover(int id)
    {
        using var conexao = Conexao.Criar();
        const string sql = "DELETE FROM categoria WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    private static Categoria Mapear(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"),
        Nome = reader.GetString("nome"),
        Descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? null : reader.GetString("descricao")
    };
}

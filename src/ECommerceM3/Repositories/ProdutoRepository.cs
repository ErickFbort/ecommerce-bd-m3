using ECommerceM3.Data;
using ECommerceM3.Models;
using MySqlConnector;

namespace ECommerceM3.Repositories;

// CRUD da entidade Produto usando ADO.NET (SQL parametrizado).
public class ProdutoRepository
{
    // CREATE: INSERT INTO produto (nome, descricao, preco, estoque, id_categoria) VALUES (...)
    public int Inserir(Produto produto)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"INSERT INTO produto (nome, descricao, preco, estoque, id_categoria)
                             VALUES (@nome, @descricao, @preco, @estoque, @categoriaId);
                             SELECT LAST_INSERT_ID();";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@nome", produto.Nome);
        cmd.Parameters.AddWithValue("@descricao", (object?)produto.Descricao ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@preco", produto.Preco);
        cmd.Parameters.AddWithValue("@estoque", produto.Estoque);
        cmd.Parameters.AddWithValue("@categoriaId", produto.CategoriaId);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // READ (todos): SELECT com JOIN para trazer o nome da categoria
    public List<Produto> ListarTodos()
    {
        var lista = new List<Produto>();
        using var conexao = Conexao.Criar();
        const string sql = @"SELECT p.id, p.nome, p.descricao, p.preco, p.estoque,
                                    p.id_categoria, c.nome AS categoria_nome
                             FROM produto p
                             INNER JOIN categoria c ON c.id = p.id_categoria
                             ORDER BY p.nome;";
        using var cmd = new MySqlCommand(sql, conexao);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            lista.Add(Mapear(reader));
        return lista;
    }

    // READ (por id): SELECT ... WHERE p.id = @id
    public Produto? BuscarPorId(int id)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"SELECT p.id, p.nome, p.descricao, p.preco, p.estoque,
                                    p.id_categoria, c.nome AS categoria_nome
                             FROM produto p
                             INNER JOIN categoria c ON c.id = p.id_categoria
                             WHERE p.id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Mapear(reader) : null;
    }

    // UPDATE: UPDATE produto SET ... WHERE id = @id
    public bool Atualizar(Produto produto)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"UPDATE produto
                             SET nome = @nome, descricao = @descricao, preco = @preco,
                                 estoque = @estoque, id_categoria = @categoriaId
                             WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@nome", produto.Nome);
        cmd.Parameters.AddWithValue("@descricao", (object?)produto.Descricao ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@preco", produto.Preco);
        cmd.Parameters.AddWithValue("@estoque", produto.Estoque);
        cmd.Parameters.AddWithValue("@categoriaId", produto.CategoriaId);
        cmd.Parameters.AddWithValue("@id", produto.Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    // DELETE: DELETE FROM produto WHERE id = @id
    public bool Remover(int id)
    {
        using var conexao = Conexao.Criar();
        const string sql = "DELETE FROM produto WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    private static Produto Mapear(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"),
        Nome = reader.GetString("nome"),
        Descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? null : reader.GetString("descricao"),
        Preco = reader.GetDecimal("preco"),
        Estoque = reader.GetInt32("estoque"),
        CategoriaId = reader.GetInt32("id_categoria"),
        CategoriaNome = reader.GetString("categoria_nome")
    };
}

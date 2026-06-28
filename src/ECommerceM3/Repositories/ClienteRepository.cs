using ECommerceM3.Data;
using ECommerceM3.Models;
using MySqlConnector;

namespace ECommerceM3.Repositories;

// CRUD da entidade Cliente usando ADO.NET (SQL escrito a mao, parametrizado).
// Os telefones (multivalorados) vivem na tabela telefone_cliente.
public class ClienteRepository
{
    // CREATE: insere o cliente e seus telefones dentro de uma transacao.
    //   INSERT INTO cliente (nome, email, cpf) VALUES (...);
    //   INSERT INTO telefone_cliente (id_cliente, numero, tipo) VALUES (...);
    public int Inserir(Cliente cliente)
    {
        using var conexao = Conexao.Criar();
        using var tx = conexao.BeginTransaction();
        try
        {
            const string sql = @"INSERT INTO cliente (nome, email, cpf)
                                 VALUES (@nome, @email, @cpf);
                                 SELECT LAST_INSERT_ID();";
            using (var cmd = new MySqlCommand(sql, conexao, tx))
            {
                cmd.Parameters.AddWithValue("@nome", cliente.Nome);
                cmd.Parameters.AddWithValue("@email", cliente.Email);
                cmd.Parameters.AddWithValue("@cpf", cliente.Cpf);
                cliente.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }

            foreach (var tel in cliente.Telefones)
                InserirTelefone(conexao, tx, cliente.Id, tel);

            tx.Commit();
            return cliente.Id;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // INSERT INTO telefone_cliente (id_cliente, numero, tipo) VALUES (...)
    private static void InserirTelefone(MySqlConnection conexao, MySqlTransaction tx, int clienteId, TelefoneCliente tel)
    {
        const string sql = @"INSERT INTO telefone_cliente (id_cliente, numero, tipo)
                             VALUES (@clienteId, @numero, @tipo);";
        using var cmd = new MySqlCommand(sql, conexao, tx);
        cmd.Parameters.AddWithValue("@clienteId", clienteId);
        cmd.Parameters.AddWithValue("@numero", tel.Numero);
        cmd.Parameters.AddWithValue("@tipo", (object?)tel.Tipo ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    // READ (todos): carrega clientes e distribui os telefones por cliente.
    public List<Cliente> ListarTodos()
    {
        var lista = new List<Cliente>();
        var porId = new Dictionary<int, Cliente>();
        using var conexao = Conexao.Criar();

        const string sqlClientes = @"SELECT id, nome, email, cpf, data_cadastro
                                     FROM cliente ORDER BY nome;";
        using (var cmd = new MySqlCommand(sqlClientes, conexao))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var c = Mapear(reader);
                lista.Add(c);
                porId[c.Id] = c;
            }
        }

        const string sqlTel = @"SELECT id, id_cliente, numero, tipo FROM telefone_cliente;";
        using (var cmd = new MySqlCommand(sqlTel, conexao))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var clienteId = reader.GetInt32("id_cliente");
                if (porId.TryGetValue(clienteId, out var c))
                    c.Telefones.Add(MapearTelefone(reader));
            }
        }
        return lista;
    }

    // READ (por id): cliente + seus telefones
    public Cliente? BuscarPorId(int id)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"SELECT id, nome, email, cpf, data_cadastro
                             FROM cliente WHERE id = @id;";
        Cliente? cliente;
        using (var cmd = new MySqlCommand(sql, conexao))
        {
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            cliente = reader.Read() ? Mapear(reader) : null;
        }

        if (cliente != null)
            cliente.Telefones = ListarTelefones(conexao, id);

        return cliente;
    }

    // SELECT * FROM telefone_cliente WHERE id_cliente = @id
    private static List<TelefoneCliente> ListarTelefones(MySqlConnection conexao, int clienteId)
    {
        var telefones = new List<TelefoneCliente>();
        const string sql = @"SELECT id, id_cliente, numero, tipo
                             FROM telefone_cliente WHERE id_cliente = @clienteId;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@clienteId", clienteId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            telefones.Add(MapearTelefone(reader));
        return telefones;
    }

    // UPDATE: atualiza apenas os dados escalares do cliente.
    public bool Atualizar(Cliente cliente)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"UPDATE cliente
                             SET nome = @nome, email = @email, cpf = @cpf
                             WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@nome", cliente.Nome);
        cmd.Parameters.AddWithValue("@email", cliente.Email);
        cmd.Parameters.AddWithValue("@cpf", cliente.Cpf);
        cmd.Parameters.AddWithValue("@id", cliente.Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    // DELETE: remove o cliente. Os telefones caem por ON DELETE CASCADE.
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
        DataCadastro = reader.GetDateTime("data_cadastro")
    };

    private static TelefoneCliente MapearTelefone(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"),
        ClienteId = reader.GetInt32("id_cliente"),
        Numero = reader.GetString("numero"),
        Tipo = reader.IsDBNull(reader.GetOrdinal("tipo")) ? null : reader.GetString("tipo")
    };
}

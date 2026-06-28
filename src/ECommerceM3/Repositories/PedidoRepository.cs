using ECommerceM3.Data;
using ECommerceM3.Models;
using MySqlConnector;

namespace ECommerceM3.Repositories;

// CRUD da entidade Pedido (e dos seus ItemPedido) usando ADO.NET.
// O valor_total e sempre derivado da soma dos itens, garantindo consistencia.
public class PedidoRepository
{
    // CREATE: insere o pedido e seus itens dentro de uma transacao.
    // SQL (resumo):
    //   INSERT INTO pedido (cliente_id, status, valor_total) VALUES (...);
    //   INSERT INTO item_pedido (pedido_id, produto_id, quantidade, preco_unitario) VALUES (...);
    //   UPDATE pedido SET valor_total = (SELECT SUM(...)) WHERE id = @id;
    public int Inserir(Pedido pedido)
    {
        using var conexao = Conexao.Criar();
        using var tx = conexao.BeginTransaction();
        try
        {
            const string sqlPedido = @"INSERT INTO pedido (cliente_id, status, valor_total)
                                       VALUES (@clienteId, @status, 0);
                                       SELECT LAST_INSERT_ID();";
            using (var cmd = new MySqlCommand(sqlPedido, conexao, tx))
            {
                cmd.Parameters.AddWithValue("@clienteId", pedido.ClienteId);
                cmd.Parameters.AddWithValue("@status", pedido.Status);
                pedido.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }

            foreach (var item in pedido.Itens)
                InserirItem(conexao, tx, pedido.Id, item);

            RecalcularTotal(conexao, tx, pedido.Id);
            tx.Commit();
            return pedido.Id;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // Adiciona um item a um pedido existente e recalcula o total.
    public void AdicionarItem(int pedidoId, ItemPedido item)
    {
        using var conexao = Conexao.Criar();
        using var tx = conexao.BeginTransaction();
        try
        {
            InserirItem(conexao, tx, pedidoId, item);
            RecalcularTotal(conexao, tx, pedidoId);
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // INSERT INTO item_pedido (...) VALUES (...)
    private static void InserirItem(MySqlConnection conexao, MySqlTransaction tx, int pedidoId, ItemPedido item)
    {
        const string sql = @"INSERT INTO item_pedido (pedido_id, produto_id, quantidade, preco_unitario)
                             VALUES (@pedidoId, @produtoId, @quantidade, @preco);";
        using var cmd = new MySqlCommand(sql, conexao, tx);
        cmd.Parameters.AddWithValue("@pedidoId", pedidoId);
        cmd.Parameters.AddWithValue("@produtoId", item.ProdutoId);
        cmd.Parameters.AddWithValue("@quantidade", item.Quantidade);
        cmd.Parameters.AddWithValue("@preco", item.PrecoUnitario);
        cmd.ExecuteNonQuery();
    }

    // UPDATE pedido SET valor_total = (SELECT SUM(quantidade*preco_unitario) ...) WHERE id = @id
    private static void RecalcularTotal(MySqlConnection conexao, MySqlTransaction tx, int pedidoId)
    {
        const string sql = @"UPDATE pedido p
                             SET p.valor_total = (
                                 SELECT COALESCE(SUM(ip.quantidade * ip.preco_unitario), 0)
                                 FROM item_pedido ip WHERE ip.pedido_id = p.id)
                             WHERE p.id = @id;";
        using var cmd = new MySqlCommand(sql, conexao, tx);
        cmd.Parameters.AddWithValue("@id", pedidoId);
        cmd.ExecuteNonQuery();
    }

    // READ (todos): SELECT com JOIN para trazer o nome do cliente
    public List<Pedido> ListarTodos()
    {
        var lista = new List<Pedido>();
        using var conexao = Conexao.Criar();
        const string sql = @"SELECT pe.id, pe.cliente_id, pe.data_pedido, pe.status,
                                    pe.valor_total, c.nome AS cliente_nome
                             FROM pedido pe
                             INNER JOIN cliente c ON c.id = pe.cliente_id
                             ORDER BY pe.data_pedido DESC;";
        using var cmd = new MySqlCommand(sql, conexao);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            lista.Add(Mapear(reader));
        return lista;
    }

    // READ (por id): pedido + seus itens
    public Pedido? BuscarPorId(int id)
    {
        using var conexao = Conexao.Criar();
        const string sql = @"SELECT pe.id, pe.cliente_id, pe.data_pedido, pe.status,
                                    pe.valor_total, c.nome AS cliente_nome
                             FROM pedido pe
                             INNER JOIN cliente c ON c.id = pe.cliente_id
                             WHERE pe.id = @id;";
        Pedido? pedido;
        using (var cmd = new MySqlCommand(sql, conexao))
        {
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            pedido = reader.Read() ? Mapear(reader) : null;
        }

        if (pedido != null)
            pedido.Itens = ListarItens(conexao, id);

        return pedido;
    }

    // SELECT dos itens de um pedido com o nome do produto (JOIN)
    private static List<ItemPedido> ListarItens(MySqlConnection conexao, int pedidoId)
    {
        var itens = new List<ItemPedido>();
        const string sql = @"SELECT ip.id, ip.pedido_id, ip.produto_id, ip.quantidade,
                                    ip.preco_unitario, pr.nome AS produto_nome
                             FROM item_pedido ip
                             INNER JOIN produto pr ON pr.id = ip.produto_id
                             WHERE ip.pedido_id = @pedidoId;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@pedidoId", pedidoId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            itens.Add(new ItemPedido
            {
                Id = reader.GetInt32("id"),
                PedidoId = reader.GetInt32("pedido_id"),
                ProdutoId = reader.GetInt32("produto_id"),
                Quantidade = reader.GetInt32("quantidade"),
                PrecoUnitario = reader.GetDecimal("preco_unitario"),
                ProdutoNome = reader.GetString("produto_nome")
            });
        }
        return itens;
    }

    // UPDATE: atualiza apenas o status do pedido (cabecalho)
    public bool AtualizarStatus(int id, string status)
    {
        using var conexao = Conexao.Criar();
        const string sql = "UPDATE pedido SET status = @status WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@status", status);
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    // DELETE: remove o pedido. Os itens caem por ON DELETE CASCADE no FK.
    public bool Remover(int id)
    {
        using var conexao = Conexao.Criar();
        const string sql = "DELETE FROM pedido WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conexao);
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    private static Pedido Mapear(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"),
        ClienteId = reader.GetInt32("cliente_id"),
        DataPedido = reader.GetDateTime("data_pedido"),
        Status = reader.GetString("status"),
        ValorTotal = reader.GetDecimal("valor_total"),
        ClienteNome = reader.GetString("cliente_nome")
    };
}

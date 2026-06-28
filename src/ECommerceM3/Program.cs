using ECommerceM3.Models;
using ECommerceM3.Repositories;
using MySqlConnector;

namespace ECommerceM3;

// Ponto de entrada: menu de console que expoe os CRUDs das 4 entidades centrais.
internal static class Program
{
    private static readonly ClienteRepository Clientes = new();
    private static readonly CategoriaRepository Categorias = new();
    private static readonly ProdutoRepository Produtos = new();
    private static readonly PedidoRepository Pedidos = new();

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var sair = false;
        while (!sair)
        {
            Console.WriteLine();
            Console.WriteLine("====== E-COMMERCE M3 - MENU PRINCIPAL ======");
            Console.WriteLine("1) Clientes");
            Console.WriteLine("2) Categorias");
            Console.WriteLine("3) Produtos");
            Console.WriteLine("4) Pedidos");
            Console.WriteLine("0) Sair");
            Console.Write("Escolha: ");

            try
            {
                switch (Console.ReadLine())
                {
                    case "1": MenuClientes(); break;
                    case "2": MenuCategorias(); break;
                    case "3": MenuProdutos(); break;
                    case "4": MenuPedidos(); break;
                    case "0": sair = true; break;
                    default: Console.WriteLine("Opcao invalida."); break;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"[ERRO DE BANCO] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] {ex.Message}");
            }
        }
        Console.WriteLine("Encerrando...");
    }

    // ============================ CLIENTES ============================
    private static void MenuClientes()
    {
        var voltar = false;
        while (!voltar)
        {
            Console.WriteLine("\n--- CLIENTES --- (1) Listar (2) Inserir (3) Atualizar (4) Remover (0) Voltar");
            Console.Write("Escolha: ");
            switch (Console.ReadLine())
            {
                case "1":
                    foreach (var c in Clientes.ListarTodos())
                    {
                        var tels = c.Telefones.Count > 0
                            ? string.Join(", ", c.Telefones.Select(t => $"{t.Numero} ({t.Tipo})"))
                            : "sem telefone";
                        Console.WriteLine($"  #{c.Id} | {c.Nome} | {c.Email} | {c.Cpf} | {tels}");
                    }
                    break;
                case "2":
                    var novo = new Cliente
                    {
                        Nome = LerTexto("Nome"),
                        Email = LerTexto("Email"),
                        Cpf = LerTexto("CPF")
                    };
                    Console.WriteLine("Adicione telefones (deixe o numero vazio para encerrar):");
                    while (true)
                    {
                        var numero = LerTexto("  Numero", permitirVazio: true);
                        if (numero.Length == 0) break;
                        novo.Telefones.Add(new TelefoneCliente
                        {
                            Numero = numero,
                            Tipo = LerTexto("  Tipo (CELULAR/RESIDENCIAL/...)", permitirVazio: true)
                        });
                    }
                    Console.WriteLine($"Cliente criado com id {Clientes.Inserir(novo)}.");
                    break;
                case "3":
                    var idAtu = LerInt("Id do cliente a atualizar");
                    var existente = Clientes.BuscarPorId(idAtu);
                    if (existente == null) { Console.WriteLine("Nao encontrado."); break; }
                    existente.Nome = LerTexto($"Nome [{existente.Nome}]", permitirVazio: true) is { Length: > 0 } n ? n : existente.Nome;
                    existente.Email = LerTexto($"Email [{existente.Email}]", permitirVazio: true) is { Length: > 0 } e ? e : existente.Email;
                    existente.Cpf = LerTexto($"CPF [{existente.Cpf}]", permitirVazio: true) is { Length: > 0 } cpf ? cpf : existente.Cpf;
                    Console.WriteLine(Clientes.Atualizar(existente) ? "Atualizado." : "Nada alterado.");
                    break;
                case "4":
                    Console.WriteLine(Clientes.Remover(LerInt("Id do cliente a remover")) ? "Removido." : "Nao encontrado.");
                    break;
                case "0": voltar = true; break;
                default: Console.WriteLine("Opcao invalida."); break;
            }
        }
    }

    // ============================ CATEGORIAS ============================
    private static void MenuCategorias()
    {
        var voltar = false;
        while (!voltar)
        {
            Console.WriteLine("\n--- CATEGORIAS --- (1) Listar (2) Inserir (3) Atualizar (4) Remover (0) Voltar");
            Console.Write("Escolha: ");
            switch (Console.ReadLine())
            {
                case "1":
                    foreach (var c in Categorias.ListarTodos())
                        Console.WriteLine($"  #{c.Id} | {c.Nome} | {c.Descricao}");
                    break;
                case "2":
                    var nova = new Categoria
                    {
                        Nome = LerTexto("Nome"),
                        Descricao = LerTexto("Descricao (opcional)", permitirVazio: true)
                    };
                    Console.WriteLine($"Categoria criada com id {Categorias.Inserir(nova)}.");
                    break;
                case "3":
                    var idAtu = LerInt("Id da categoria a atualizar");
                    var existente = Categorias.BuscarPorId(idAtu);
                    if (existente == null) { Console.WriteLine("Nao encontrada."); break; }
                    existente.Nome = LerTexto($"Nome [{existente.Nome}]", permitirVazio: true) is { Length: > 0 } n ? n : existente.Nome;
                    existente.Descricao = LerTexto($"Descricao [{existente.Descricao}]", permitirVazio: true) is { Length: > 0 } d ? d : existente.Descricao;
                    Console.WriteLine(Categorias.Atualizar(existente) ? "Atualizada." : "Nada alterado.");
                    break;
                case "4":
                    Console.WriteLine(Categorias.Remover(LerInt("Id da categoria a remover")) ? "Removida." : "Nao encontrada.");
                    break;
                case "0": voltar = true; break;
                default: Console.WriteLine("Opcao invalida."); break;
            }
        }
    }

    // ============================ PRODUTOS ============================
    private static void MenuProdutos()
    {
        var voltar = false;
        while (!voltar)
        {
            Console.WriteLine("\n--- PRODUTOS --- (1) Listar (2) Inserir (3) Atualizar (4) Remover (0) Voltar");
            Console.Write("Escolha: ");
            switch (Console.ReadLine())
            {
                case "1":
                    foreach (var p in Produtos.ListarTodos())
                        Console.WriteLine($"  #{p.Id} | {p.Nome} | R$ {p.Preco:N2} | estoque {p.Estoque} | {p.CategoriaNome}");
                    break;
                case "2":
                    var novo = new Produto
                    {
                        Nome = LerTexto("Nome"),
                        Descricao = LerTexto("Descricao (opcional)", permitirVazio: true),
                        Preco = LerDecimal("Preco"),
                        Estoque = LerInt("Estoque"),
                        CategoriaId = LerInt("Id da categoria")
                    };
                    Console.WriteLine($"Produto criado com id {Produtos.Inserir(novo)}.");
                    break;
                case "3":
                    var idAtu = LerInt("Id do produto a atualizar");
                    var existente = Produtos.BuscarPorId(idAtu);
                    if (existente == null) { Console.WriteLine("Nao encontrado."); break; }
                    existente.Nome = LerTexto($"Nome [{existente.Nome}]", permitirVazio: true) is { Length: > 0 } n ? n : existente.Nome;
                    existente.Descricao = LerTexto($"Descricao [{existente.Descricao}]", permitirVazio: true) is { Length: > 0 } d ? d : existente.Descricao;
                    existente.Preco = LerDecimal($"Preco [{existente.Preco:N2}]", permitirVazio: true) ?? existente.Preco;
                    existente.Estoque = LerInt($"Estoque [{existente.Estoque}]", permitirVazio: true) ?? existente.Estoque;
                    existente.CategoriaId = LerInt($"Id categoria [{existente.CategoriaId}]", permitirVazio: true) ?? existente.CategoriaId;
                    Console.WriteLine(Produtos.Atualizar(existente) ? "Atualizado." : "Nada alterado.");
                    break;
                case "4":
                    Console.WriteLine(Produtos.Remover(LerInt("Id do produto a remover")) ? "Removido." : "Nao encontrado.");
                    break;
                case "0": voltar = true; break;
                default: Console.WriteLine("Opcao invalida."); break;
            }
        }
    }

    // ============================ PEDIDOS ============================
    private static void MenuPedidos()
    {
        var voltar = false;
        while (!voltar)
        {
            Console.WriteLine("\n--- PEDIDOS --- (1) Listar (2) Detalhar (3) Inserir (4) Atualizar status (5) Remover (0) Voltar");
            Console.Write("Escolha: ");
            switch (Console.ReadLine())
            {
                case "1":
                    foreach (var p in Pedidos.ListarTodos())
                        Console.WriteLine($"  #{p.Id} | {p.ClienteNome} | {p.DataPedido:dd/MM/yyyy} | {p.Status} | R$ {p.ValorTotal:N2}");
                    break;
                case "2":
                    var det = Pedidos.BuscarPorId(LerInt("Id do pedido"));
                    if (det == null) { Console.WriteLine("Nao encontrado."); break; }
                    Console.WriteLine($"Pedido #{det.Id} | Cliente: {det.ClienteNome} | {det.Status} | Total R$ {det.ValorTotal:N2}");
                    foreach (var it in det.Itens)
                        Console.WriteLine($"    - {it.Quantidade}x {it.ProdutoNome} (R$ {it.PrecoUnitario:N2}) = R$ {it.Subtotal:N2}");
                    break;
                case "3":
                    var novo = new Pedido { ClienteId = LerInt("Id do cliente") };
                    Console.WriteLine("Adicione itens (deixe o id do produto vazio para encerrar):");
                    while (true)
                    {
                        var produtoId = LerInt("  Id do produto", permitirVazio: true);
                        if (produtoId == null) break;
                        novo.Itens.Add(new ItemPedido
                        {
                            ProdutoId = produtoId.Value,
                            Quantidade = LerInt("  Quantidade"),
                            PrecoUnitario = LerDecimal("  Preco unitario")
                        });
                    }
                    if (novo.Itens.Count == 0) { Console.WriteLine("Pedido sem itens, cancelado."); break; }
                    Console.WriteLine($"Pedido criado com id {Pedidos.Inserir(novo)}.");
                    break;
                case "4":
                    var idStatus = LerInt("Id do pedido");
                    var status = LerTexto("Novo status (PENDENTE/PROCESSANDO/ENVIADO/ENTREGUE/CANCELADO)");
                    Console.WriteLine(Pedidos.AtualizarStatus(idStatus, status) ? "Status atualizado." : "Nao encontrado.");
                    break;
                case "5":
                    Console.WriteLine(Pedidos.Remover(LerInt("Id do pedido a remover")) ? "Removido." : "Nao encontrado.");
                    break;
                case "0": voltar = true; break;
                default: Console.WriteLine("Opcao invalida."); break;
            }
        }
    }

    // ============================ HELPERS DE ENTRADA ============================
    private static string LerTexto(string rotulo, bool permitirVazio = false)
    {
        while (true)
        {
            Console.Write($"{rotulo}: ");
            var entrada = Console.ReadLine()?.Trim() ?? string.Empty;
            if (permitirVazio || entrada.Length > 0) return entrada;
            Console.WriteLine("Valor obrigatorio.");
        }
    }

    private static int LerInt(string rotulo) => LerInt(rotulo, permitirVazio: false)!.Value;

    private static int? LerInt(string rotulo, bool permitirVazio)
    {
        while (true)
        {
            Console.Write($"{rotulo}: ");
            var entrada = Console.ReadLine()?.Trim() ?? string.Empty;
            if (permitirVazio && entrada.Length == 0) return null;
            if (int.TryParse(entrada, out var valor)) return valor;
            Console.WriteLine("Informe um numero inteiro valido.");
        }
    }

    private static decimal LerDecimal(string rotulo) => LerDecimal(rotulo, permitirVazio: false)!.Value;

    private static decimal? LerDecimal(string rotulo, bool permitirVazio)
    {
        while (true)
        {
            Console.Write($"{rotulo}: ");
            var entrada = Console.ReadLine()?.Trim().Replace(',', '.') ?? string.Empty;
            if (permitirVazio && entrada.Length == 0) return null;
            if (decimal.TryParse(entrada, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var valor)) return valor;
            Console.WriteLine("Informe um valor numerico valido.");
        }
    }
}

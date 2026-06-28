# Trabalho M3 - Banco de Dados: e-Commerce

Projeto da disciplina de Banco de Dados (UNIVALI - Politecnica/NEI).
Dominio: **e-Commerce** (Mercado Livre / Amazon).

Implementa um CRUD de console em **C# (.NET 10)** com acesso ao **MySQL** via
**ADO.NET (SQL puro)**, usando a biblioteca `MySqlConnector`.

## Entidades (modelo relacional)

- **cliente** (1) --- (N) **pedido**
- **categoria** (1) --- (N) **produto**
- **pedido** (1) --- (N) **item_pedido** (N) --- (1) **produto**

A tabela `item_pedido` resolve o relacionamento N:N entre `pedido` e `produto`.

## Pre-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/) (`dotnet --version`)
- Um servidor MySQL. A forma recomendada e via **Docker** (ver abaixo).

## 1. Subir o banco com Docker (recomendado)

O arquivo [`docker-compose.yml`](docker-compose.yml) ja sobe o MySQL 8 na porta 3306,
cria o banco `ecommerce_m3` e executa automaticamente os scripts de `sql/` na primeira
inicializacao (cria as tabelas e popula os dados de exemplo).

```bash
docker compose up -d        # sobe o MySQL e roda 01_schema.sql + 02_seed.sql
docker compose ps           # confere se o container esta "Up"
```

Usuario: `root` | Senha: `senha123` | Banco: `ecommerce_m3` (definidos no compose).

Comandos uteis:

```bash
docker compose down         # para o container (mantem os dados no volume)
docker compose down -v      # para e APAGA os dados (recria do zero no proximo up)
```

> Alternativa sem Docker: rode os scripts manualmente num MySQL local
> (MySQL Workbench, DBeaver ou `mysql -u root -p < sql/01_schema.sql` e depois
> `sql/02_seed.sql`).

## 2. Configurar a conexao

A connection string em [`src/ECommerceM3/Data/Conexao.cs`](src/ECommerceM3/Data/Conexao.cs)
ja vem alinhada com o `docker-compose.yml`:

```csharp
"Server=localhost;Port=3306;Database=ecommerce_m3;User ID=root;Password=senha123;"
```

Ajuste apenas se usar outro MySQL/senha/porta.

## 3. Executar a aplicacao

```bash
cd src/ECommerceM3
dotnet run
```

Um menu de console sera exibido com os CRUDs das quatro entidades centrais
(Clientes, Categorias, Produtos e Pedidos).

## Estrutura do projeto

```
ECommerceM3/
├── docker-compose.yml       # sobe o MySQL 8 e roda os scripts de sql/ no 1o boot
├── sql/
│   ├── 01_schema.sql        # CREATE DATABASE + tabelas (PK/FK/constraints)
│   └── 02_seed.sql          # INSERTs de dados de exemplo
└── src/ECommerceM3/
    ├── Data/Conexao.cs      # fabrica de conexao com o MySQL
    ├── Models/              # POCOs das entidades
    ├── Repositories/        # CRUD por entidade (SQL parametrizado)
    └── Program.cs           # menu de console
```

> Observacao: o acesso a dados usa SQL escrito manualmente (sem ORM). Cada metodo
> de repositorio traz o comando SQL correspondente proximo a implementacao.

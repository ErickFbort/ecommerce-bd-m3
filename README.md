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
- Servidor MySQL em execucao (MySQL Server, XAMPP, Docker, etc.)

## 1. Criar e popular o banco

Execute os scripts SQL na ordem abaixo (via MySQL Workbench, DBeaver ou linha de comando):

```bash
mysql -u root -p < sql/01_schema.sql   # cria o schema e as tabelas
mysql -u root -p < sql/02_seed.sql      # popula com dados de exemplo
```

## 2. Configurar a conexao

Edite a connection string em [`src/ECommerceM3/Data/Conexao.cs`](src/ECommerceM3/Data/Conexao.cs)
com o usuario e a senha do seu MySQL:

```csharp
"Server=localhost;Port=3306;Database=ecommerce_m3;User ID=root;Password=SUA_SENHA;"
```

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

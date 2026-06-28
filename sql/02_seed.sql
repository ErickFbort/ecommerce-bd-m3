-- =====================================================================
-- Trabalho M3 - Banco de Dados (UNIVALI)
-- Dominio: e-Commerce (Mercado Livre / Amazon)
-- Povoamento das tabelas (dados de exemplo)
-- Executar APOS o script 01_schema.sql
-- =====================================================================

USE ecommerce_m3;

-- Limpa dados existentes para permitir reexecucao do seed
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE item_pedido;
TRUNCATE TABLE pedido;
TRUNCATE TABLE produto;
TRUNCATE TABLE categoria;
TRUNCATE TABLE telefone_cliente;
TRUNCATE TABLE cliente;
SET FOREIGN_KEY_CHECKS = 1;

-- ---------------------------------------------------------------------
-- Categorias
-- ---------------------------------------------------------------------
INSERT INTO categoria (id, nome, descricao) VALUES
    (1, 'Eletronicos',    'Celulares, computadores e acessorios'),
    (2, 'Livros',         'Livros fisicos e didaticos'),
    (3, 'Casa e Cozinha', 'Utilidades domesticas e eletrodomesticos'),
    (4, 'Moda',           'Roupas, calcados e acessorios'),
    (5, 'Games',          'Consoles, jogos e perifericos');

-- ---------------------------------------------------------------------
-- Produtos
-- ---------------------------------------------------------------------
INSERT INTO produto (id, nome, descricao, preco, estoque, categoria_id) VALUES
    (1,  'Smartphone Galaxy A55', 'Tela 6.6", 256GB, 8GB RAM',          1899.90,  30, 1),
    (2,  'Notebook Ideapad 3',    'Ryzen 5, 16GB RAM, SSD 512GB',       3299.00,  15, 1),
    (3,  'Fone Bluetooth JBL',    'Fone intra-auricular sem fio',        249.90,  80, 1),
    (4,  'Livro: Banco de Dados', 'Sistemas de Banco de Dados, 7a ed.',  189.50,  40, 2),
    (5,  'Livro: Clean Code',     'Codigo limpo - Robert C. Martin',     129.90,  25, 2),
    (6,  'Air Fryer 4L',          'Fritadeira sem oleo 1500W',           399.00,  20, 3),
    (7,  'Jogo de Panelas Inox',  'Conjunto 5 pecas',                    279.90,  18, 3),
    (8,  'Tenis Esportivo',       'Tenis para corrida unissex',          219.90,  60, 4),
    (9,  'Camiseta Basica',       'Camiseta algodao 100%',                49.90, 120, 4),
    (10, 'Controle Xbox Series',  'Controle sem fio',                    449.00,  35, 5);

-- ---------------------------------------------------------------------
-- Clientes
-- ---------------------------------------------------------------------
INSERT INTO cliente (id, nome, email, cpf, data_cadastro) VALUES
    (1, 'Ana Souza',     'ana.souza@email.com',     '111.111.111-11', '2026-01-10 09:30:00'),
    (2, 'Bruno Lima',    'bruno.lima@email.com',    '222.222.222-22', '2026-02-05 14:10:00'),
    (3, 'Carla Mendes',  'carla.mendes@email.com',  '333.333.333-33', '2026-02-20 18:45:00'),
    (4, 'Diego Rocha',   'diego.rocha@email.com',   '444.444.444-44', '2026-03-12 11:00:00'),
    (5, 'Elaine Castro', 'elaine.castro@email.com', '555.555.555-55', '2026-04-01 08:20:00');

-- ---------------------------------------------------------------------
-- Telefones dos clientes (atributo multivalorado). A Ana possui 2 numeros.
-- ---------------------------------------------------------------------
INSERT INTO telefone_cliente (cliente_id, numero, tipo) VALUES
    (1, '(47) 99911-1111', 'CELULAR'),
    (1, '(47) 3344-1111',  'RESIDENCIAL'),
    (2, '(47) 99922-2222', 'CELULAR'),
    (3, '(47) 99933-3333', 'CELULAR'),
    (4, '(47) 99944-4444', 'CELULAR'),
    (5, '(47) 99955-5555', 'CELULAR');

-- ---------------------------------------------------------------------
-- Pedidos
-- O valor_total e definido como 0 aqui e recalculado a partir dos itens
-- pelo UPDATE no final do script (garante consistencia com item_pedido).
-- ---------------------------------------------------------------------
INSERT INTO pedido (id, cliente_id, data_pedido, status, valor_total) VALUES
    (1, 1, '2026-05-02 10:15:00', 'ENTREGUE',    0.00),
    (2, 2, '2026-05-10 16:40:00', 'ENVIADO',     0.00),
    (3, 3, '2026-05-15 13:05:00', 'PENDENTE',    0.00),
    (4, 1, '2026-06-01 09:50:00', 'ENTREGUE',    0.00),
    (5, 4, '2026-06-20 19:25:00', 'PROCESSANDO', 0.00);

-- ---------------------------------------------------------------------
-- Itens dos pedidos (tabela associativa)
-- ---------------------------------------------------------------------
INSERT INTO item_pedido (pedido_id, produto_id, quantidade, preco_unitario) VALUES
    -- Pedido 1: Smartphone + Fone
    (1, 1, 1, 1899.90),
    (1, 3, 1,  249.90),
    -- Pedido 2: Notebook
    (2, 2, 1, 3299.00),
    -- Pedido 3: Livro BD + Tenis + Camiseta
    (3, 4, 1,  189.50),
    (3, 8, 1,  219.90),
    (3, 9, 1,   49.90),
    -- Pedido 4: Air Fryer + Livro Clean Code
    (4, 6, 1,  399.00),
    (4, 5, 1,  129.90),
    -- Pedido 5: Controle Xbox
    (5, 10, 1, 449.00);

-- ---------------------------------------------------------------------
-- Recalcula valor_total de cada pedido a partir dos seus itens
-- ---------------------------------------------------------------------
UPDATE pedido p
SET p.valor_total = (
    SELECT COALESCE(SUM(ip.quantidade * ip.preco_unitario), 0)
    FROM item_pedido ip
    WHERE ip.pedido_id = p.id
);

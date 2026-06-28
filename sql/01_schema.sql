-- =====================================================================
-- Trabalho M3 - Banco de Dados (UNIVALI)
-- Dominio: e-Commerce (Mercado Livre / Amazon)
-- Projeto Fisico - Criacao do esquema e tabelas (MySQL)
-- =====================================================================

-- Cria o banco de dados (schema) e seleciona-o para uso
CREATE DATABASE IF NOT EXISTS ecommerce_m3
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE ecommerce_m3;

-- Remove as tabelas caso ja existam (ordem inversa das dependencias),
-- permitindo recriar o esquema do zero sem erros de chave estrangeira.
DROP TABLE IF EXISTS item_pedido;
DROP TABLE IF EXISTS pedido;
DROP TABLE IF EXISTS produto;
DROP TABLE IF EXISTS categoria;
DROP TABLE IF EXISTS telefone_cliente;
DROP TABLE IF EXISTS cliente;

-- ---------------------------------------------------------------------
-- Tabela: cliente
-- Entidade central que representa o comprador da plataforma.
-- ---------------------------------------------------------------------
CREATE TABLE cliente (
    id            INT          NOT NULL AUTO_INCREMENT,
    nome          VARCHAR(120) NOT NULL,
    email         VARCHAR(120) NOT NULL,
    cpf           VARCHAR(14)  NOT NULL,
    data_cadastro DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_cliente PRIMARY KEY (id),
    CONSTRAINT uq_cliente_email UNIQUE (email),
    CONSTRAINT uq_cliente_cpf   UNIQUE (cpf)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Tabela: telefone_cliente
-- Telefone e um atributo MULTIVALORADO do cliente; pela 1FN ele e
-- extraido para uma tabela propria (um cliente pode ter varios telefones).
-- Relacionamento non-identifying: a FK cliente_id NAO faz parte da PK.
-- ---------------------------------------------------------------------
CREATE TABLE telefone_cliente (
    id         INT         NOT NULL AUTO_INCREMENT,
    cliente_id INT         NOT NULL,
    numero     VARCHAR(20) NOT NULL,
    tipo       VARCHAR(20) NULL,
    CONSTRAINT pk_telefone_cliente PRIMARY KEY (id),
    CONSTRAINT fk_telefone_cliente_cliente
        FOREIGN KEY (cliente_id) REFERENCES cliente (id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Tabela: categoria
-- Agrupa os produtos por tipo (ex.: Eletronicos, Livros).
-- ---------------------------------------------------------------------
CREATE TABLE categoria (
    id        INT          NOT NULL AUTO_INCREMENT,
    nome      VARCHAR(80)  NOT NULL,
    descricao VARCHAR(255) NULL,
    CONSTRAINT pk_categoria PRIMARY KEY (id),
    CONSTRAINT uq_categoria_nome UNIQUE (nome)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Tabela: produto
-- Item vendido na plataforma. Pertence a uma categoria (1-N).
-- ---------------------------------------------------------------------
CREATE TABLE produto (
    id           INT            NOT NULL AUTO_INCREMENT,
    nome         VARCHAR(150)   NOT NULL,
    descricao    VARCHAR(500)   NULL,
    preco        DECIMAL(10,2)  NOT NULL,
    estoque      INT            NOT NULL DEFAULT 0,
    categoria_id INT            NOT NULL,
    CONSTRAINT pk_produto PRIMARY KEY (id),
    CONSTRAINT fk_produto_categoria
        FOREIGN KEY (categoria_id) REFERENCES categoria (id),
    CONSTRAINT ck_produto_preco   CHECK (preco >= 0),
    CONSTRAINT ck_produto_estoque CHECK (estoque >= 0)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Tabela: pedido
-- Compra realizada por um cliente (1-N). Reune varios itens.
-- ---------------------------------------------------------------------
CREATE TABLE pedido (
    id          INT            NOT NULL AUTO_INCREMENT,
    cliente_id  INT            NOT NULL,
    data_pedido DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    status      VARCHAR(30)    NOT NULL DEFAULT 'PENDENTE',
    valor_total DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
    CONSTRAINT pk_pedido PRIMARY KEY (id),
    CONSTRAINT fk_pedido_cliente
        FOREIGN KEY (cliente_id) REFERENCES cliente (id),
    CONSTRAINT ck_pedido_valor CHECK (valor_total >= 0)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Tabela: item_pedido (associativa N-N entre pedido e produto)
-- Resolve o relacionamento muitos-para-muitos guardando a quantidade
-- e o preco unitario praticado no momento da compra.
-- ---------------------------------------------------------------------
-- A chave primaria e COMPOSTA pelas FKs das duas entidades (pedido_id, produto_id),
-- caracterizando relacionamentos identifying (a tabela filha nao existe sem as maes).
CREATE TABLE item_pedido (
    pedido_id      INT            NOT NULL,
    produto_id     INT            NOT NULL,
    quantidade     INT            NOT NULL,
    preco_unitario DECIMAL(10,2)  NOT NULL,
    CONSTRAINT pk_item_pedido PRIMARY KEY (pedido_id, produto_id),
    CONSTRAINT fk_item_pedido_pedido
        FOREIGN KEY (pedido_id) REFERENCES pedido (id) ON DELETE CASCADE,
    CONSTRAINT fk_item_pedido_produto
        FOREIGN KEY (produto_id) REFERENCES produto (id),
    CONSTRAINT ck_item_quantidade CHECK (quantidade > 0),
    CONSTRAINT ck_item_preco      CHECK (preco_unitario >= 0)
) ENGINE=InnoDB;

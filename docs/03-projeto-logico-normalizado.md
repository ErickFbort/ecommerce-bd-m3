# Projeto Lógico Normalizado — e-Commerce

Estrutura lógica do banco de dados em formato textual, com a justificativa de que o
esquema atende à **3ª Forma Normal (3FN)**. Notação: `tabela (chave_primaria, demais_atributos)`,
com a chave primária em destaque e as chaves estrangeiras indicadas logo abaixo de cada relação.

## Estrutura lógica (relações)

```
cliente (id, nome, email, cpf, data_cadastro)

telefone_cliente (id, cliente_id, numero, tipo)
    cliente_id referencia cliente(id)

categoria (id, nome, descricao)

produto (id, categoria_id, nome, descricao, preco, estoque)
    categoria_id referencia categoria(id)

pedido (id, cliente_id, data_pedido, status, valor_total)
    cliente_id referencia cliente(id)

item_pedido (pedido_id, produto_id, quantidade, preco_unitario)
    pedido_id  referencia pedido(id)
    produto_id referencia produto(id)
```

> Observação: `item_pedido` possui **chave primária composta** (`pedido_id`, `produto_id`),
> caracterizando os relacionamentos com `pedido` e `produto` como *identifying*.

## 1ª Forma Normal (1FN)

> Regra: a tabela não pode ter **atributos multivalorados** nem **atributos compostos**.

- Todas as colunas armazenam um único valor atômico por linha.
- O telefone do cliente é, por natureza, **multivalorado** (um cliente pode ter vários
  números). Por isso ele **não** ficou como coluna em `cliente`; foi extraído para a
  tabela própria `telefone_cliente`, onde cada número é uma linha. Isso elimina a
  violação da 1FN.
- Não há atributos compostos (não há, por exemplo, um campo "endereço" agregando
  rua/número/cidade — caso existisse, seria decomposto em uma tabela própria).

**Conclusão:** todas as tabelas estão na 1FN.

## 2ª Forma Normal (2FN)

> Regra: estar na 1FN **e** todo atributo não-chave depender da **chave primária inteira**
> (não pode haver dependência funcional **parcial** em chaves compostas).

- A única tabela com chave composta é `item_pedido` (`pedido_id`, `produto_id`). Seus
  atributos não-chave dependem da **combinação** dos dois:
  - `quantidade`: a quantidade só faz sentido para um produto **dentro de** um pedido
    específico → depende de (`pedido_id`, `produto_id`).
  - `preco_unitario`: é o preço **praticado naquele pedido** (preço histórico no momento
    da compra), que pode diferir do `preco` atual em `produto`. Logo depende dos dois e
    **não** é dependência parcial de `produto_id`.
- As demais tabelas têm chave primária **simples** (`id`), então não há possibilidade de
  dependência parcial.

**Conclusão:** todas as tabelas estão na 2FN.

## 3ª Forma Normal (3FN)

> Regra: estar na 2FN **e** não existir **dependência funcional transitiva** entre
> atributos não-chave (atributo não-chave dependendo de outro atributo não-chave).

- `produto` guarda apenas `categoria_id` (a FK), e **não** o nome da categoria. O nome da
  categoria depende de `categoria_id`, então mantê-lo em `produto` criaria a transitiva
  `produto.id → categoria_id → categoria.nome`. Como isso foi evitado (o nome fica só em
  `categoria`), não há dependência transitiva.
- `pedido` guarda `cliente_id`, e não os dados do cliente (nome, email…), que ficam apenas
  em `cliente`. Sem transitiva.
- Nas demais tabelas, todos os atributos não-chave descrevem diretamente a própria
  entidade identificada pela PK.

**Conclusão:** todas as tabelas estão na 3FN.

## Observação sobre `valor_total` (pedido)

`pedido.valor_total` é um valor **derivável** da soma de `quantidade * preco_unitario`
dos itens do pedido. Tecnicamente é um dado redundante, mas foi mantido de forma
**controlada** por desempenho (evita recalcular a soma a cada consulta) — uma flexibilização
prevista na bibliografia (Heuser/Elmasri) quando há ganho de performance. A consistência é
garantida pela aplicação, que recalcula o total sempre que os itens mudam.

## Resultado

O projeto lógico está na **3ª Forma Normal (3FN)**: sem atributos multivalorados/compostos
(1FN), sem dependências parciais (2FN) e sem dependências transitivas (3FN).

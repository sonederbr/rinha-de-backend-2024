CREATE OR REPLACE FUNCTION atualiza_saldo_cliente_and_insere_transacao(
    p_id_cliente INT,
    p_valor INT,
    p_descricao VARCHAR(10),
    p_tipo CHAR(1))
    RETURNS TABLE (saldo_atualizado INT, limite_atual INT, linhas_afetadas INT)
AS $$
    DECLARE
        v_saldo INT = 0;
        v_limite INT = 0;
        v_linhas_afetadas INT = 0;
BEGIN

    PERFORM pg_advisory_xact_lock(p_id_cliente);
    SELECT saldo, limite INTO v_saldo, v_limite FROM saldo_cliente WHERE id_cliente = p_id_cliente;

    IF p_tipo = 'c' THEN
        v_saldo = v_saldo + p_valor;
        UPDATE saldo_cliente SET saldo = v_saldo WHERE id_cliente = p_id_cliente;
        GET diagnostics v_linhas_afetadas = row_count;
    ELSE
        v_saldo = v_saldo - p_valor;
        UPDATE saldo_cliente SET saldo = v_saldo WHERE id_cliente = p_id_cliente AND abs(saldo - p_valor) <= limite;
        GET diagnostics v_linhas_afetadas = row_count;
    END IF;

    IF v_linhas_afetadas > 0 THEN
        INSERT INTO transacao_cliente(id_cliente, valor, tipo, descricao, realizada_em)
        VALUES (p_id_cliente, p_valor, p_tipo, p_descricao, NOW());
    END IF;

    -- RETURN QUERY SELECT v_saldo, v_limite, v_linhas_afetadas;
    RETURN QUERY SELECT saldo AS saldo_atualizado, limite AS limite_atual, v_linhas_afetadas AS linhas_afetadas FROM saldo_cliente WHERE id_cliente = p_id_cliente;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION obter_extrato_cliente(p_id_cliente INT)
    RETURNS TABLE (
        limite INT,
        valor INT,
        tipo CHAR(1),
        descricao VARCHAR(10),
        data TIMESTAMP)
    LANGUAGE sql
AS $$
    (SELECT limite       AS limite
          , saldo        AS valor
          , NULL         AS tipo
          , NULL         AS descricao
          , NOW()        AS data
       FROM saldo_cliente
      WHERE id_cliente = p_id_cliente
      LIMIT 1)
    UNION ALL
    (SELECT NULL         AS limite
          , valor        AS valor
          , tipo         AS tipo
          , descricao    AS descricao
          , realizada_em AS data
       FROM transacao_cliente
      WHERE id_cliente = p_id_cliente
      ORDER BY id DESC
      LIMIT 10)
$$;



-- 
-- 
-- CREATE OR REPLACE FUNCTION credita_saldo_cliente_e_insere_transacao(
--     p_id_cliente INT,
--     p_valor INT,
--     p_descricao VARCHAR(10))
--     RETURNS TABLE (
--                       saldo_atualizado INT,
--                       limite_atual INT,
--                       linhas_afetadas INT)
--     LANGUAGE plpgsql
-- AS $$
-- BEGIN
--     PERFORM pg_advisory_xact_lock(p_id_cliente);
-- 
--     INSERT INTO transacao_cliente(id_cliente, valor, tipo, descricao, realizada_em)
--     VALUES (p_id_cliente, p_valor, 'c', p_descricao, NOW());
-- 
--     RETURN QUERY
--         UPDATE saldo_cliente
--             SET saldo = saldo + p_valor
--             WHERE id_cliente = p_id_cliente
--             RETURNING saldo, limite, 1;
-- END;
-- $$;
-- 
-- 
-- CREATE OR REPLACE FUNCTION debita_saldo_cliente_e_insere_transacao(
--     p_id_cliente INT,
--     p_valor INT,
--     p_descricao VARCHAR(10))
--     RETURNS TABLE (
--                       saldo_atualizado INT,
--                       limite_atual INT,
--                       linhas_afetadas INT)
--     LANGUAGE plpgsql
-- AS $$
-- DECLARE
--     saldo_atual int;
--     limite_atual int;
-- 
-- BEGIN
--     PERFORM pg_advisory_xact_lock(p_id_cliente);
-- 
--     SELECT
--         limite,
--         COALESCE(saldo, 0)
--     INTO
--         limite_atual,
--         saldo_atual
--     FROM saldo_cliente
--     WHERE id_cliente = p_id_cliente;
-- 
--     IF saldo_atual - p_valor >= limite_atual * -1 THEN
--         INSERT INTO transacao_cliente(id_cliente, valor, tipo, descricao, realizada_em)
--         VALUES (p_id_cliente, p_valor, 'd', p_descricao, NOW());
-- 
--         UPDATE saldo_cliente
--         SET saldo = saldo - p_valor
--         WHERE id_cliente = p_id_cliente;
-- 
--         RETURN QUERY
--             SELECT
--                 saldo,
--                 limite,
--                 1
--             FROM saldo_cliente
--             WHERE id_cliente = p_id_cliente;
--     ELSE
--         RETURN QUERY
--             SELECT
--                 saldo,
--                 limite,
--                 0
--             FROM saldo_cliente
--             WHERE id_cliente = p_id_cliente;
--     END IF;
-- END;
-- $$;

-- SELECT sc.saldo
--      , sc.limite
--      , NOW() AS data_extrato
--      , tc.valor
--      , tc.tipo
--      , tc.descricao
--      , tc.realizada_em
--   FROM saldo_cliente sc
--   LEFT JOIN transacao_cliente tc ON sc.id_cliente = tc.id_cliente
--  WHERE sc.id_cliente = p_id_cliente
--  ORDER BY tc.id DESC
--  LIMIT 10;

-- CREATE OR REPLACE FUNCTION atualiza_saldo_cliente_and_insere_transacao(
--     p_id_cliente INT,
--     p_valor INT,
--     p_tipo CHAR(1),
--     p_descricao VARCHAR(10)
-- ) RETURNS TABLE(novo_saldo INT, novo_limite INT, crebitou INT) AS $$
-- DECLARE
--     v_saldo INT;
--     v_limite INT;
--     v_row_count INT = 0;
-- 
-- BEGIN
--     IF p_valor = 0 THEN
--         return;
--     END IF;
-- 
--     PERFORM pg_advisory_xact_lock(p_cliente_id);
-- 
--     SELECT saldo, limite INTO v_saldo, v_limite FROM clientes WHERE id = p_cliente_id;
-- 
--     IF p_tipo = 'd' THEN
--         v_saldo = v_saldo - p_valor;
--         UPDATE clientes SET saldo = saldo - p_valor WHERE id = p_cliente_id AND abs(saldo - p_valor) <= limite;
--         GET diagnostics v_row_count = row_count;
--     ELSIF p_tipo = 'c' THEN
--         v_saldo = v_saldo + p_valor;
--         UPDATE clientes SET saldo = saldo + p_valor WHERE id = p_cliente_id;
--         GET diagnostics v_row_count = row_count;
--     END IF;
-- 
--     IF v_row_count > 0 THEN
--         INSERT INTO transacoes(valor, tipo, descricao, realizada_em, idcliente)
--         VALUES (p_valor, p_tipo, p_descricao, (now() AT TIME ZONE 'UTC'), p_cliente_id);
--     END IF;
-- 
--     RETURN QUERY SELECT v_saldo, v_limite, v_row_count;
-- END;
-- $$ LANGUAGE plpgsql;


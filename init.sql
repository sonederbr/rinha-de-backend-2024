SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

CREATE UNLOGGED TABLE cliente (
     id integer PRIMARY KEY NOT NULL,
     saldo integer NOT NULL,
     limite integer NOT NULL
);

CREATE UNIQUE INDEX idx_idcliente
    ON cliente(id);

CLUSTER cliente USING idx_idcliente;

CREATE UNLOGGED TABLE transacao (
   id SERIAL PRIMARY KEY,
   valor integer NOT NULL,
   tipo char(1) NOT NULL,
   descricao varchar(250) NOT NULL,
   realizada_em timestamp NOT NULL,
   idcliente integer NOT NULL
);

-- CREATE INDEX idx_transacao_idcliente 
-- ON transacao (idcliente);
-- CLUSTER transacao USING idx_transacao_idcliente;

CREATE OR REPLACE FUNCTION atualiza_saldo_cliente_and_insere_transacao(
    p_cliente_id INT,
    p_valor INT,
    p_tipo VARCHAR(1),
    p_descricao VARCHAR(10)
) RETURNS TABLE(novo_saldo INT, novo_limite INT, crebitou INT) AS $$
DECLARE
    v_saldo INT;
    v_limite INT;
    v_row_count INT = 0;

BEGIN
    IF p_valor = 0 THEN
        return;
    END IF;

    LOCK TABLE cliente IN ROW EXCLUSIVE MODE;
    SELECT saldo, limite INTO v_saldo, v_limite FROM cliente WHERE id = p_cliente_id FOR NO KEY UPDATE;

    IF p_tipo = 'd' THEN
        v_saldo = v_saldo - p_valor;
        UPDATE cliente SET saldo = saldo - p_valor WHERE id = p_cliente_id AND abs(saldo - p_valor) <= limite;
        GET diagnostics v_row_count = row_count;
    ELSIF p_tipo = 'c' THEN
        v_saldo = v_saldo + p_valor;
        UPDATE cliente SET saldo = saldo + p_valor WHERE id = p_cliente_id;
        GET diagnostics v_row_count = row_count;
    END IF;

    IF v_row_count > 0 THEN
        INSERT INTO transacao(valor, tipo, descricao, realizada_em, idcliente)
        VALUES (p_valor, p_tipo, p_descricao, (now() AT TIME ZONE 'UTC'), p_cliente_id);
    END IF;

    RETURN QUERY SELECT v_saldo, v_limite, v_row_count;
END; $$ LANGUAGE plpgsql;

INSERT INTO cliente (id, saldo, limite) VALUES (1, 0, 100000);
INSERT INTO cliente (id, saldo, limite) VALUES (2, 0, 80000);
INSERT INTO cliente (id, saldo, limite) VALUES (3, 0, 1000000);
INSERT INTO cliente (id, saldo, limite) VALUES (4, 0, 10000000);
INSERT INTO cliente (id, saldo, limite) VALUES (5, 0, 500000);

-- 1	100000	0
-- 2	80000	0
-- 3	1000000	0
-- 4	10000000	0
-- 5	500000	0
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

CREATE TABLE cliente (
    id integer PRIMARY KEY NOT NULL,
    saldo integer NOT NULL,
    limite integer NOT NULL
);

CREATE UNIQUE INDEX idx_idcliente
    ON cliente(id);

CLUSTER cliente USING idx_idcliente;

CREATE TABLE transacao (
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


CREATE OR REPLACE FUNCTION fc_obter_transacoes(p_cliente_id integer)
RETURNS TABLE (
    id integer,
    valor integer,
    tipo char(1),
    descricao varchar(250),
    realizada_em timestamp,
	idcliente integer
)
AS $$
BEGIN
RETURN QUERY (
    SELECT 
		t.id,
        t.valor,
        t.tipo,
        t.descricao,
        t.realizada_em,
        t.idcliente
            FROM 
            transacao AS t
        WHERE 
            t.idcliente = p_cliente_id
        ORDER BY 
            t.realizada_em DESC
        LIMIT 10
       );
END; $$
LANGUAGE plpgsql;

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
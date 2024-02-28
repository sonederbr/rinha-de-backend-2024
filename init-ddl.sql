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

CREATE UNLOGGED TABLE saldo_cliente (
    id SERIAL PRIMARY KEY NOT NULL,
    id_cliente INTEGER NOT NULL,
    saldo INTEGER NOT NULL,
    limite INTEGER NOT NULL
);
CREATE UNIQUE INDEX idx_cliente_id_cliente ON saldo_cliente (id_cliente);

CREATE UNLOGGED TABLE transacao_cliente (
    id SERIAL PRIMARY KEY,
    id_cliente INTEGER NOT NULL,
    valor INTEGER NOT NULL,
    tipo CHAR(1) NOT NULL,
    descricao VARCHAR(10) NOT NULL,
    realizada_em TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);
CREATE INDEX idx_transacao_cliente_id_cliente ON transacao_cliente (id_cliente);


INSERT INTO saldo_cliente (id_cliente, saldo, limite)
VALUES (1, 0,   1000 * 100),
       (2, 0,    800 * 100),
       (3, 0,  10000 * 100),
       (4, 0, 100000 * 100),
       (5, 0,   5000 * 100);
END;
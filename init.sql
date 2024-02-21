CREATE UNLOGGED TABLE clientes (
    id integer PRIMARY KEY NOT NULL,
    saldo integer NOT NULL,
    limite integer NOT NULL
);
CREATE UNIQUE INDEX idx_idcliente ON clientes (id);

CREATE UNLOGGED TABLE transacoes (
    id SERIAL PRIMARY KEY,
    valor integer NOT NULL,
    tipo char(1) NOT NULL,
    descricao varchar(250) NOT NULL,
    realizada_em timestamp NOT NULL,
    idcliente integer NOT NULL
);
CREATE INDEX idx_transacao_idcliente ON transacoes (idcliente);

DO $$
    BEGIN
        INSERT INTO clientes (id, saldo, limite)
        VALUES (1, 0,   1000 * 100),
               (2, 0,    800 * 100),
               (3, 0,  10000 * 100),
               (4, 0, 100000 * 100),
               (5, 0,   5000 * 100);
    END;
$$;

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

    PERFORM pg_advisory_xact_lock(p_cliente_id);

    SELECT saldo, limite INTO v_saldo, v_limite FROM clientes WHERE id = p_cliente_id;

    IF p_tipo = 'd' THEN
        v_saldo = v_saldo - p_valor;
        UPDATE clientes SET saldo = saldo - p_valor WHERE id = p_cliente_id AND abs(saldo - p_valor) <= limite;
        GET diagnostics v_row_count = row_count;
    ELSIF p_tipo = 'c' THEN
        v_saldo = v_saldo + p_valor;
        UPDATE clientes SET saldo = saldo + p_valor WHERE id = p_cliente_id;
        GET diagnostics v_row_count = row_count;
    END IF;

    IF v_row_count > 0 THEN
        INSERT INTO transacoes(valor, tipo, descricao, realizada_em, idcliente)
        VALUES (p_valor, p_tipo, p_descricao, (now() AT TIME ZONE 'UTC'), p_cliente_id);
    END IF;

    RETURN QUERY SELECT v_saldo, v_limite, v_row_count;
END;
$$ LANGUAGE plpgsql;


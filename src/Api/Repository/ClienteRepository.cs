using Api.Model;
using Npgsql;

namespace Api.Repository;

public  class ClienteRepository
{
    private readonly NpgsqlDataSource _datasource;
    private readonly NpgsqlConnection _connection;

    public ClienteRepository(NpgsqlDataSource datasource, NpgsqlConnection connection)
    {
        _datasource = datasource;
        _connection = connection;
    }
    
    public virtual async Task<Cliente?> ObterClienteAsync(int id, CancellationToken ct = default)
    {
        Cliente? cliente = null;
        await using var cmd = _datasource.CreateCommand();
        cmd.CommandText = @"SELECT id, limite, saldo
                              FROM cliente
                             WHERE id = $1
                             LIMIT 1;";

        cmd.Parameters.AddWithValue(id);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            cliente ??= new Cliente(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2));
        }

        return cliente;
    }
    
    public virtual async Task<Cliente?> ObterExtratoAsync(int id, CancellationToken ct = default)
    {
        Cliente? cliente = null;

        await using (_connection)
        {
            await _connection.OpenAsync(ct);

            await using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT c.id           AS Id
                                         , c.limite       AS Limite
                                         , c.saldo        AS Saldo
                                         , t.descricao    AS Descricao
                                         , t.valor        AS Valor
                                         , t.tipo         AS Tipo
                                         , t.realizada_em AS DataTransacao
                                      FROM cliente c
                                      LEFT JOIN transacao t ON t.idcliente = c.id
                                     WHERE c.id = $1
                                     ORDER BY t.realizada_em DESC
                                     LIMIT 10;";

                cmd.Parameters.AddWithValue(id);

                await using (var reader = await cmd.ExecuteReaderAsync(ct))
                {
                    while (await reader.ReadAsync())
                    {
                        cliente ??= new Cliente(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2));

                        if(!reader.IsDBNull(3))
                            cliente.Transacoes.Add(new Transacao(
                                descricao: reader.GetString(3),
                                valor: reader.GetInt32(4),
                                tipo: reader.GetString(5)));
                    }
                }
            }
            return cliente;
        }
    }
    
    public virtual async Task<IReadOnlyCollection<Cliente>> ObterTodosClientesAsync(CancellationToken ct = default)
    {
        var clientes = new List<Cliente>();
        await using var cmd = _datasource.CreateCommand();
        cmd.CommandText = @"SELECT id, limite, saldo
                              FROM cliente
                             ORDER BY id ASC ;";

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            clientes.Add(new Cliente(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2)));
        }
        return clientes.AsReadOnly();
    }
}
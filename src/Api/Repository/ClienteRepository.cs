using Api.Endpoints.Cliente.Dtos;
using Api.Model;
using Npgsql;

namespace Api.Repository;

public class ClienteRepository
{
    private readonly NpgsqlDataSource _datasource;
    private readonly NpgsqlConnection _connection;

    public ClienteRepository(NpgsqlDataSource datasource, NpgsqlConnection connection)
    {
        _datasource = datasource;
        _connection = connection;
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
                    while (await reader.ReadAsync(ct))
                    {
                        cliente ??= new Cliente(
                            Id: reader.GetInt32(0),
                            Limite: reader.GetInt32(1),
                            Saldo: reader.GetInt32(2));

                        if(!reader.IsDBNull(3))
                            cliente.Value.Add(new Transacao(
                                Descricao: reader.GetString(3),
                                Valor: reader.GetInt32(4),
                                Tipo: reader.GetString(5)));
                    }
                }
            }
            return cliente;
        }
    }

    public virtual async Task<ClienteModel?> CrebitarAsync(Crebitar crebitar, CancellationToken ct = default)
    {
        await using var cmd = _datasource.CreateCommand();
        cmd.CommandText = @"SELECT novo_saldo, novo_limite, crebitou
                              FROM atualiza_saldo_cliente_and_insere_transacao($1, $2, $3, $4);";

        cmd.Parameters.AddWithValue(crebitar.IdCliente);
        cmd.Parameters.AddWithValue(crebitar.Valor);
        cmd.Parameters.AddWithValue(crebitar.Tipo);
        cmd.Parameters.AddWithValue(crebitar.Descricao);
        
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            if (reader.GetInt32(2) > 0)
            {
                return new ClienteModel
                {
                    Limite = reader.GetInt32(0),
                    Saldo = reader.GetInt32(1)
                };
            }
        }
        return null;
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
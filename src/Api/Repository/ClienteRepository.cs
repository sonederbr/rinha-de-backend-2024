using Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Repository;

public sealed class ClienteRepository(RinhaDbContext rinhaDbContext) : BaseRepository(rinhaDbContext)
{
    private readonly RinhaDbContext _rinhaDbContext = rinhaDbContext;

    public async Task<Cliente?> ObterCliente(int id, CancellationToken cancellationToken = default)
    {
        return await _rinhaDbContext.Clientes.FindAsync(id, cancellationToken);
    }

    public async Task<Cliente?> ObterClienteTransacao(int id, CancellationToken cancellationToken = default)
    {
        _rinhaDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        var query = await _rinhaDbContext.QueryAsync<dynamic>(cancellationToken,
            text: @"SELECT 
                     c.id           AS Id, 
                     c.saldo        AS Saldo, 
                     c.limite       AS Limite, 
                     t.id           AS Transacoes_Id,
                     t.valor        AS Transacoes_Valor,
                     t.tipo         AS Transacoes_Tipo, 
                     t.descricao    AS Transacoes_Descricao, 
                     t.realizada_em AS Transacoes_DataTransacao, 
                     t.idcliente    AS Transacoes_ClienteId
                  FROM cliente c
                  LEFT JOIN transacao t ON t.idcliente = c.id
                 WHERE c.id = @Id
                 ORDER BY t.realizada_em DESC
                 LIMIT 10;",
            parameters: new
            {
                Id = id
            });
        
        Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(Cliente), new List<string> { "Id" });
        Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(Transacao), new List<string> { "Id" });

        var cliente = (Slapper.AutoMapper.MapDynamic<Cliente>(query) as IEnumerable<Cliente>).ToList();

        return cliente?.First();
    }

    public Task Atualizar(Cliente entity, CancellationToken cancellationToken = default)
    {
        _rinhaDbContext.Set<Cliente>().Update(entity);
        return Task.CompletedTask;
    }
}
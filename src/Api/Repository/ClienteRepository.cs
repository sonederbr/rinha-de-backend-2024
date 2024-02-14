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
        var cliente =  await _rinhaDbContext.Clientes.FindAsync(id, cancellationToken);
        if (cliente is null)
            return null;

        cliente.Transacoes = _rinhaDbContext.Transacoes
            .FromSql($"SELECT * FROM fc_obter_transacoes({id})").ToList();

        return cliente;
    }
    
    public Task Atualizar(Cliente entity, CancellationToken cancellationToken = default)
    {
        _rinhaDbContext.Set<Cliente>().Update(entity);
        return Task.CompletedTask;
    }
}
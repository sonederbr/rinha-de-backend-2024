using Api.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace Api.Repository;

public class RinhaRepository(RinhaContext rinhaContext)
{
    public virtual ValueTask<Cliente?> GetCliente(int id)
    {
        return rinhaContext.Clientes.FindAsync(id);
    }
    
    public virtual async Task<IReadOnlyCollection<Transacao>> GetTransacoes(int id)
    {
        return await rinhaContext.Transacoes
            .Take(10)
            .Where(p => p.ClienteId == id)
            .OrderByDescending(p => p.DataTransacao)
            .ToListAsync();
    }

    public virtual async Task<int> AtualizarCliente(Cliente cliente)
    {
        return await rinhaContext.SaveChangesAsync();
    }

    public virtual async Task<int> NovaTransacao(Transacao transacao)
    {
        await rinhaContext.Transacoes.AddAsync(transacao);
        return await rinhaContext.SaveChangesAsync();
    }
}
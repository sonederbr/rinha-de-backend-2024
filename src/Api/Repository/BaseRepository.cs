namespace Api.Repository;

public abstract class BaseRepository(RinhaDbContext rinhaDbContext)
{
    public virtual async Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return await rinhaDbContext.SaveChangesAsync(cancellationToken);
    }
}
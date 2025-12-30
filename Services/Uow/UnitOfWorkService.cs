namespace apiBozzi.Services.Uow;

public class UnitOfWorkService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    public async Task ExecuteInTransactionAsync(Func<Task> work, CancellationToken cancellationToken = default)
    {
        await using var tx = await Context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await work();
            await Context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
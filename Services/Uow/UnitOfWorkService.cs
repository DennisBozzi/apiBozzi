using apiBozzi.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace apiBozzi.Services.Uow;

public class UnitOfWorkService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }

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

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> work, CancellationToken cancellationToken = default)
    {
        await using var tx = await Context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await work();
            await Context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
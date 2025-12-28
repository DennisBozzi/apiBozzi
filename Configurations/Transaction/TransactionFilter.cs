using Microsoft.AspNetCore.Mvc.Filters;
using apiBozzi.Services.Uow;

namespace apiBozzi.Configurations.Transaction;

public class TransactionFilter(UnitOfWorkService unitOfWorkService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await unitOfWorkService.ExecuteInTransactionAsync(async () =>
        {
            var executedContext = await next();

            if (executedContext.Exception != null && !executedContext.ExceptionHandled)
                throw executedContext.Exception;
        });
    }
}
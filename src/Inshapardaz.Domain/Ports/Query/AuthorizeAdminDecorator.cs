using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Paramore.Darker;
using Paramore.Darker.Attributes;

namespace Inshapardaz.Domain.Ports.Query;

public class AuthorizeAdminDecorator<TQuery, TResult>(IUserHelper userHelper) : IQueryHandlerDecorator<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public IQueryContext Context { get; set; }

    public void InitializeFromAttributeParams(object[] attributeParams)
    {
    }

    public TResult Execute(TQuery query, Func<TQuery, TResult> next, Func<TQuery, TResult> fallback)
    {
        var result = next(query);
        return result;
    }

    public async Task<TResult> ExecuteAsync(TQuery query,
        Func<TQuery, CancellationToken, Task<TResult>> next,
        Func<TQuery, CancellationToken, Task<TResult>> fallback,
        CancellationToken cancellationToken = default)
    {
        var account = userHelper.Account;
        var isAuthenticated = userHelper.IsAuthenticated;

        if (!isAuthenticated || !account.IsSuperAdmin)
        {
            throw new UnauthorizedException();
        }

        return await next(query, cancellationToken).ConfigureAwait(false);

    }
}

public sealed class AuthorizeAdminAttribute(int step) : QueryHandlerAttribute(step)
{
    public override object[] GetAttributeParams() => Array.Empty<object>();

    public override Type GetDecoratorType() => typeof(AuthorizeAdminDecorator<,>);
}

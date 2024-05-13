using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Paramore.Darker;
using Paramore.Darker.Attributes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query;

public class AuthorizeAdminDecorator<TQuery, TResult> : IQueryHandlerDecorator<TQuery, TResult>
   where TQuery : IQuery<TResult>
{
    private readonly IUserHelper _userHelper;

    public AuthorizeAdminDecorator(IUserHelper userHelper)
    {
        _userHelper = userHelper;
    }

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
        var account = _userHelper.Account;
        var isAuthenticated = _userHelper.IsAuthenticated;

        if (!isAuthenticated || !account.IsSuperAdmin)
        {
            throw new UnauthorizedException();
        }

        return await next(query, cancellationToken).ConfigureAwait(false);

    }
}

public sealed class AuthorizeAdminAttribute : QueryHandlerAttribute
{
    public AuthorizeAdminAttribute(int step)
        : base(step)
    {
    }

    public override object[] GetAttributeParams()
    {
        return Array.Empty<object>();
    }

    public override Type GetDecoratorType()
    {
        return typeof(AuthorizeAdminDecorator<,>);
    }
}

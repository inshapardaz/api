using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Paramore.Darker;
using Paramore.Darker.Attributes;

namespace Inshapardaz.Domain.Ports.Query.Library;

public class LibraryAuthorizeDecorator<TQuery, TResult>(IUserHelper userHelper, ILibraryRepository libraryRepository)
    : IQueryHandlerDecorator<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private Role[] _roles;

    public IQueryContext Context { get; set; }

    public void InitializeFromAttributeParams(object[] attributeParams) => _roles = (Role[])attributeParams[0];

    public TResult Execute(TQuery query, Func<TQuery, TResult> next, Func<TQuery, TResult> fallback)
    {
        var libraryQuery = (LibraryBaseQuery<TResult>)(IQuery<TResult>)query;

        var account = userHelper.Account;
        var isAuthenticated = userHelper.IsAuthenticated;


        if (!isAuthenticated)
        {
            throw new UnauthorizedException();

        }

        var libraries = libraryRepository.GetLibrariesByAccountId(account.Id).GetAwaiter().GetResult();
        var library = libraries.SingleOrDefault(l => l.Id == libraryQuery.LibraryId);

        if (account.IsSuperAdmin)
        {
            return next(query);

        }
        else if (!_roles.Any() && isAuthenticated)
        {
            return next(query);

        }
        else if (library != null && _roles.Contains(library.Role))
        {
            return next(query);

        }

        throw new ForbiddenException();

    }

    public async Task<TResult> ExecuteAsync(TQuery query,
        Func<TQuery, CancellationToken, Task<TResult>> next,
        Func<TQuery, CancellationToken, Task<TResult>> fallback,
        CancellationToken cancellationToken = default)
    {
        var libraryQuery = (LibraryBaseQuery<TResult>)(IQuery<TResult>)query;

        var account = userHelper.Account;
        var isAuthenticated = userHelper.IsAuthenticated;
        if (!isAuthenticated)
        {
            throw new UnauthorizedException();

        }

        if (account.IsSuperAdmin)
        {
            return await next(query, cancellationToken);
        }

        var libraries = await libraryRepository.GetLibrariesByAccountId(account.Id);
        var library = libraries.SingleOrDefault(l => l.Id == libraryQuery.LibraryId);

        if (!_roles.Any() && isAuthenticated)
        {
            return await next(query, cancellationToken);
        }
        else if (library != null && _roles.Contains(library.Role))
        {
            return await next(query, cancellationToken);
        }

        throw new ForbiddenException();
    }
}

public sealed class LibraryAuthorizeAttribute(int step, params Role[] roles) : QueryHandlerAttribute(step)
{
    public override object[] GetAttributeParams() => new[] { roles };

    public override Type GetDecoratorType() => typeof(LibraryAuthorizeDecorator<,>);
}

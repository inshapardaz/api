using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library;


public class LibraryAuthorizeHandler<TRequest>(IUserHelper userHelper, ILibraryRepository libraryRepository)
    : RequestHandlerAsync<TRequest>
    where TRequest : LibraryBaseCommand, IRequest
{
    private HandlerTiming _timing;
    private Role[] _roles;

    public override void InitializeFromAttributeParams(
        params object[] initializerList
    )
    {
        _timing = (HandlerTiming)initializerList[0];
        _roles = (Role[])initializerList[1];
    }

    public override Task<TRequest> HandleAsync(TRequest command, CancellationToken cancellationToken = default)
    {
        var account = userHelper.Account;
        var isAuthenticated = userHelper.IsAuthenticated;

        if (!isAuthenticated)
        {
            throw new UnauthorizedException();
        }

        var libraries = libraryRepository.GetLibrariesByAccountId(account.Id).Result;
        var library = libraries.SingleOrDefault(l => l.Id == command.LibraryId);

        if (account.IsSuperAdmin)
        {
            return base.HandleAsync(command, cancellationToken);
        }
        else if (!_roles.Any() && isAuthenticated)
        {
            return base.HandleAsync(command, cancellationToken);
        }
        else if (library != null && _roles.Contains(library.Role))
        {
            return base.HandleAsync(command, cancellationToken);
        }

        throw new ForbiddenException();
    }

}

public class LibraryAuthorizeAttribute(int step, params Role[] roles) : RequestHandlerAttribute(step)
{
    public override object[] InitializerParams() => new object[] { Timing, roles };

    public override Type GetHandlerType() => typeof(LibraryAuthorizeHandler<>);
}

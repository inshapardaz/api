using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library;


public class LibraryAuthorizeHandler<TRequest>
    : RequestHandlerAsync<TRequest> where TRequest : LibraryBaseCommand, IRequest
{
    private HandlerTiming _timing;
    private Role[] _roles;

    private readonly IUserHelper _userHelper;
    private readonly ILibraryRepository _libraryRepository;

    public LibraryAuthorizeHandler(IUserHelper userHelper, ILibraryRepository libraryRepository)
    {
        _userHelper = userHelper;
        _libraryRepository = libraryRepository;
    }
    public override void InitializeFromAttributeParams(
        params object[] initializerList
    )
    {
        _timing = (HandlerTiming)initializerList[0];
        _roles = (Role[])initializerList[1];
    }

    public override Task<TRequest> HandleAsync(TRequest command, CancellationToken cancellationToken = default)
    {
        var account = _userHelper.Account;
        var isAuthenticated = _userHelper.IsAuthenticated;

        if (!isAuthenticated)
        {
            throw new UnauthorizedException();
        }

        var libraries = _libraryRepository.GetLibrariesByAccountId(account.Id).Result;
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

public class LibraryAuthorizeAttribute : RequestHandlerAttribute
{
    private Role[] _roles;

    public LibraryAuthorizeAttribute(int step, params Role[] roles)
        : base(step)
    {
        _roles = roles;
    }

    public override object[] InitializerParams()
    {
        return new object[] { Timing, _roles };
    }

    public override Type GetHandlerType()
    {
        return typeof(LibraryAuthorizeHandler<>);
    }
}

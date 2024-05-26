﻿using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command;


public class AuthorizeAdminHandler<TRequest>
    : RequestHandlerAsync<TRequest> where TRequest : class, IRequest
{
    private HandlerTiming _timing;

    private readonly IUserHelper _userHelper;

    public AuthorizeAdminHandler(IUserHelper userHelper)
    {
        _userHelper = userHelper;
    }

    public override void InitializeFromAttributeParams(
        params object[] initializerList
    )
    {
        _timing = (HandlerTiming)initializerList[0];
    }

    public override Task<TRequest> HandleAsync(TRequest command, CancellationToken cancellationToken = default)
    {
        var account = _userHelper.Account;
        var isAuthenticated = _userHelper.IsAuthenticated;

        if (!isAuthenticated)
        {
            throw new UnauthorizedException();
        }

        if (!account.IsSuperAdmin)
        {
            throw new ForbiddenException();
        }

        return base.HandleAsync(command, cancellationToken);
    }

}

public class AuthorizeAdminAttribute : RequestHandlerAttribute
{
    public AuthorizeAdminAttribute(int step)
        : base(step)
    {
    }

    public override object[] InitializerParams()
    {
        return new object[] { Timing };
    }

    public override Type GetHandlerType()
    {
        return typeof(AuthorizeAdminHandler<>);
    }
}

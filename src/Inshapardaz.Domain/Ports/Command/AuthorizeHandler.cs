using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command;


public class AuthorizeHandler<TRequest>(IUserHelper userHelper) : RequestHandlerAsync<TRequest>
    where TRequest : class, IRequest
{
    private HandlerTiming _timing;

    public override void InitializeFromAttributeParams(
        params object[] initializerList
    ) =>
        _timing = (HandlerTiming)initializerList[0];

    public override Task<TRequest> HandleAsync(TRequest command, CancellationToken cancellationToken = default)
    {
        var account = userHelper.Account;
        var isAuthenticated = userHelper.IsAuthenticated;

        if (!isAuthenticated)
        {
            throw new UnauthorizedException();
        }

        return base.HandleAsync(command, cancellationToken);
    }

}

public class AuthorizeAttribute(int step) : RequestHandlerAttribute(step)
{
    public override object[] InitializerParams() => new object[] { Timing };

    public override Type GetHandlerType() => typeof(AuthorizeHandler<>);
}

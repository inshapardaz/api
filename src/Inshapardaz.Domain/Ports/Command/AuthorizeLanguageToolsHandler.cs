using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command;

// Allows Admins and DictionaryWriters: the accounts that manage the language tools.
public class AuthorizeLanguageToolsHandler<TRequest>(IUserHelper userHelper) : RequestHandlerAsync<TRequest>
    where TRequest : class, IRequest
{
    private HandlerTiming _timing;

    public override void InitializeFromAttributeParams(
        params object[] initializerList
    ) =>
        _timing = (HandlerTiming)initializerList[0];

    public override Task<TRequest> HandleAsync(TRequest command, CancellationToken cancellationToken = default)
    {
        if (!userHelper.IsAuthenticated)
        {
            throw new UnauthorizedException();
        }

        if (!userHelper.CanManageLanguageTools)
        {
            throw new ForbiddenException();
        }

        return base.HandleAsync(command, cancellationToken);
    }
}

public class AuthorizeLanguageToolsAttribute(int step) : RequestHandlerAttribute(step)
{
    public override object[] InitializerParams() => new object[] { Timing };

    public override Type GetHandlerType() => typeof(AuthorizeLanguageToolsHandler<>);
}

using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command;

// Allows any editor: Admin, DictionaryWriter, or Writer/LibraryAdmin in at least one library.
public class AuthorizeEditorHandler<TRequest>(IUserHelper userHelper) : RequestHandlerAsync<TRequest>
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

        if (!userHelper.IsEditor)
        {
            throw new ForbiddenException();
        }

        return base.HandleAsync(command, cancellationToken);
    }
}

public class AuthorizeEditorAttribute(int step) : RequestHandlerAttribute(step)
{
    public override object[] InitializerParams() => new object[] { Timing };

    public override Type GetHandlerType() => typeof(AuthorizeEditorHandler<>);
}

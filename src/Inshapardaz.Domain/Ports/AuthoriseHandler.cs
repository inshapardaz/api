using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports
{
    public class AuthoriseHandler<T>
        : RequestHandlerAsync<T> where T : LibraryAuthorisedCommand
    {
        private readonly IReadClaims _claimsReader;

        public AuthoriseHandler(IReadClaims claimsReader)
        {
            _claimsReader = claimsReader;
        }

        public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (command.Claims == null)
            {
                throw new UnauthorizedException();
            }

            if (!_claimsReader.IsWriter(command.Claims))
            {
                throw new ForbiddenException("Bearer");
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }

    public class AuthoriseAttribute : RequestHandlerAttribute
    {
        public AuthoriseAttribute(int step, HandlerTiming timing)
            : base(step, timing)
        { }

        public override object[] InitializerParams()
        {
            return new object[] { Timing };
        }

        public override Type GetHandlerType()
        {
            return typeof(AuthoriseHandler<>);
        }
    }
}

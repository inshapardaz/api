using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Handlers.Library;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models
{
    public class LibraryAuthoriseHandler<T>
        : RequestHandlerAsync<T> where T : LibraryAuthorisedCommand
    {
        private readonly IUserHelper _userHelper;

        public LibraryAuthoriseHandler(IUserHelper claimsReader)
        {
            _userHelper = claimsReader;
        }

        public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO: Check if user has permissions to this particular library
            //if (command.Claims == null)
            //{
            //    throw new UnauthorizedException();
            //}

            //if (!_userHelper.IsAdmin())
            //{
            //    throw new ForbiddenException("Bearer");
            //}

            return await base.HandleAsync(command, cancellationToken);
        }
    }

    public class LibraryAdminAuthoriseAttribute : RequestHandlerAttribute
    {
        public LibraryAdminAuthoriseAttribute(int step, HandlerTiming timing)
            : base(step, timing)
        { }

        public override object[] InitializerParams()
        {
            return new object[] { Timing };
        }

        public override Type GetHandlerType()
        {
            return typeof(LibraryAuthoriseHandler<>);
        }
    }
}

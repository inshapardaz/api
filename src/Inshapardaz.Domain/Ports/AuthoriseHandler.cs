using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models
{
    public class AuthoriseHandler<T>
        : RequestHandlerAsync<T> where T : RequestBase
    {
        private readonly IUserHelper _userHelper;
        public Permission[] _permissions;

        public AuthoriseHandler(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public override void InitializeFromAttributeParams(params object[] initializerList)
        {
            _permissions = initializerList[0] as Permission[];
        }

        public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (!_userHelper.IsAuthenticated)
            {
                throw new UnauthorizedException();
            }

            //if (!_userHelper.CheckPermissions(_permissions))
            //{
            //    throw new ForbiddenException("Bearer");
            //}

            return await base.HandleAsync(command, cancellationToken);
        }
    }

    public class AuthoriseAttribute : RequestHandlerAttribute
    {
        public Permission[] Permissions { get; }

        public AuthoriseAttribute(int step = 0, HandlerTiming timing = HandlerTiming.Before, params Permission[] permissions)
            : base(step, timing)
        {
            Permissions = permissions;
        }

        public override object[] InitializerParams()
        {
            return new object[] { Permissions };
        }

        public override Type GetHandlerType()
        {
            return typeof(AuthoriseHandler<>);
        }
    }
}

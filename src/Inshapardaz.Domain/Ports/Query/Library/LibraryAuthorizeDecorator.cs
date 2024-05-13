using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Paramore.Darker;
using Paramore.Darker.Attributes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Handlers.Library;

namespace Inshapardaz.Domain.Ports.Query
{
    public class LibraryAuthorizeDecorator<TQuery, TResult> : IQueryHandlerDecorator<TQuery, TResult>
       where TQuery : IQuery<TResult>
    {
        private readonly IUserHelper _userHelper;
        private readonly ILibraryRepository _libraryRepository;
        private Role[] _roles;

        public LibraryAuthorizeDecorator(IUserHelper userHelper, ILibraryRepository libraryRepository)
        {
            _userHelper = userHelper;
            _libraryRepository = libraryRepository;
        }

        public IQueryContext Context { get; set; }

        public void InitializeFromAttributeParams(object[] attributeParams)
        {
            _roles = (Role[])attributeParams[0];
        }

        public TResult Execute(TQuery query, Func<TQuery, TResult> next, Func<TQuery, TResult> fallback)
        {
            var libraryQuery = (LibraryBaseQuery<TResult>)(IQuery<TResult>)query;

            var account = _userHelper.Account;
            var isAuthenticated = _userHelper.IsAuthenticated;
            

            if (!isAuthenticated)
            {
                throw new UnauthorizedException();

            }

            var libraries = _libraryRepository.GetLibrariesByAccountId(account.Id).Result;
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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var libraryQuery = (LibraryBaseQuery<TResult>)(IQuery<TResult>)query;

            var account = _userHelper.Account;
            var isAuthenticated = _userHelper.IsAuthenticated;
            if (!isAuthenticated)
            {
                throw new UnauthorizedException();

            }

            if (account.IsSuperAdmin)
            {
                return await next(query, cancellationToken);
            }
            
            var libraries = await _libraryRepository.GetLibrariesByAccountId(account.Id);
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

    public sealed class LibraryAuthorizeAttribute : QueryHandlerAttribute
    {
        private Role[] _roles;

        public LibraryAuthorizeAttribute(int step, params Role[] roles)
            : base(step)
        {
            _roles = roles;
        }

        public override object[] GetAttributeParams()
        {
            return new[] { _roles };
        }

        public override Type GetDecoratorType()
        {
            return typeof(LibraryAuthorizeDecorator<,>);
        }
    }

}

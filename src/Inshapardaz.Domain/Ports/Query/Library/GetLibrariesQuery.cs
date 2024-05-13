using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Query;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library
{
    public class GetLibrariesQuery : IQuery<Page<LibraryModel>>
    {
        public GetLibrariesQuery(int pageNumber, int pageSize, int? accountId = null, bool isAdmin = false)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            AccountId = accountId;
            IsAdmin = isAdmin;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
        public int? AccountId { get; }
        public bool IsAdmin { get; }
        public string Query { get; set; }
    }

    public class GetLibrariesQueryHandler : QueryHandlerAsync<GetLibrariesQuery, Page<LibraryModel>>
    {
        private readonly ILibraryRepository _libraryRepository;

        public GetLibrariesQueryHandler(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public override async Task<Page<LibraryModel>> ExecuteAsync(GetLibrariesQuery query, CancellationToken cancellationToken = default)
        {
            if (query.AccountId.HasValue)
            {
                if (query.IsAdmin)
                {
                    return string.IsNullOrWhiteSpace(query.Query)
                        ? await _libraryRepository.GetLibraries(query.PageNumber, query.PageSize, cancellationToken)
                        : await _libraryRepository.FindLibraries(query.Query, query.PageNumber, query.PageSize, cancellationToken);
                }
                else
                {
                    return string.IsNullOrWhiteSpace(query.Query)
                    ? await _libraryRepository.GetUserLibraries(query.AccountId.Value, query.PageNumber, query.PageSize, cancellationToken)
                    : await _libraryRepository.FindUserLibraries(query.Query, query.AccountId.Value, query.PageNumber, query.PageSize, cancellationToken);
                }
            }
            return string.IsNullOrWhiteSpace(query.Query)
                ? await _libraryRepository.GetPublicLibraries(query.PageNumber, query.PageSize, cancellationToken)
                : await _libraryRepository.FindPublicLibraries(query.Query, query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}

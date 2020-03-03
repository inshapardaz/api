using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetFilesByBookQuery : LibraryAuthorisedQuery<IEnumerable<FileModel>>
    {
        public GetFilesByBookQuery(int libraryId, int bookId, Guid userId)
            : base(libraryId, userId)
        {
            BookId = bookId;
        }

        public int BookId { get; set; }
    }

    public class GetFilesByBookQueryHandler : QueryHandlerAsync<GetFilesByBookQuery, IEnumerable<FileModel>>
    {
        private readonly IBookRepository _bookRepository;

        public GetFilesByBookQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<IEnumerable<FileModel>> ExecuteAsync(GetFilesByBookQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.UserId, cancellationToken);
            if (book != null)
            {
                var files = await _bookRepository.GetFilesByBook(command.BookId, cancellationToken);

                return files != null ? files : Enumerable.Empty<FileModel>();
            }

            return null;
        }
    }
}

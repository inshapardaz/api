using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetFilesByBookQuery : IQuery<IEnumerable<FileModel>>
    {
        public GetFilesByBookQuery(int bookId, Guid userId)
        {
            UserId = userId;
            BookId = bookId;
        }

        public int BookId { get; set; }

        public Guid UserId { get; }
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
            var book = await _bookRepository.GetBookById(command.BookId, cancellationToken);
            if (book != null)
            {
                var files = await _bookRepository.GetFilesByBook(command.BookId, cancellationToken);

                return files != null ? files : Enumerable.Empty<FileModel>();
            }

            return null;
        }
    }
}


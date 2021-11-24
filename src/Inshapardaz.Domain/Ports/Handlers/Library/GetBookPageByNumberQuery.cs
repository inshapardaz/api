using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetBookPageByNumberQuery : LibraryBaseQuery<BookPageModel>
    {
        public GetBookPageByNumberQuery(int libraryId, int bookId, int sequenceNumber)
            : base(libraryId)
        {
            BookId = bookId;
            SequenceNumber = sequenceNumber;
        }

        public int BookId { get; set; }
        public int SequenceNumber { get; }
    }

    public class GetBookPageByNumberQueryHandler : QueryHandlerAsync<GetBookPageByNumberQuery, BookPageModel>
    {
        private readonly IBookPageRepository _bookPageRepository;

        public GetBookPageByNumberQueryHandler(IBookPageRepository bookPageRepository)
        {
            _bookPageRepository = bookPageRepository;
        }

        public override async Task<BookPageModel> ExecuteAsync(GetBookPageByNumberQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var page = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
            if (page != null)
            {
                var previousPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber - 1, cancellationToken);
                var nextPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber + 1, cancellationToken);

                page.PreviousPage = previousPage;
                page.NextPage = nextPage;
            }

            return page;
        }
    }
}

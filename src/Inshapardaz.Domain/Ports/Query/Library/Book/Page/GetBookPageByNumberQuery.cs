using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Page;

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
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public GetBookPageByNumberQueryHandler(IBookPageRepository bookPageRepository, 
        IFileRepository fileRepository, 
        IFileStorage fileStorage)
    {
        _bookPageRepository = bookPageRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    public override async Task<BookPageModel> ExecuteAsync(GetBookPageByNumberQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var page = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);

        if (page != null)
        {
            if (page.ContentId.HasValue)
            {
                var file = await _fileRepository.GetFileById(page.ContentId.Value, cancellationToken);
                if (file != null)
                {
                    var fc = await _fileStorage.GetTextFile(file.FilePath, cancellationToken);
                    page.Text = fc;
                }
            }
            var previousPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber - 1, cancellationToken);
            var nextPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber + 1, cancellationToken);

            page.PreviousPage = previousPage;
            page.NextPage = nextPage;
        }

        return page;
    }
}

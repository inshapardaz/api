using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Page;

public class GetBookPageByNumberQuery(int libraryId, int bookId, int sequenceNumber)
    : LibraryBaseQuery<BookPageModel>(libraryId)
{
    public int BookId { get; set; } = bookId;
    public int SequenceNumber { get; } = sequenceNumber;
}

public class GetBookPageByNumberQueryHandler(
    IBookPageRepository bookPageRepository,
    IFileRepository fileRepository,
    IFileStorage fileStorage)
    : QueryHandlerAsync<GetBookPageByNumberQuery, BookPageModel>
{
    public override async Task<BookPageModel> ExecuteAsync(GetBookPageByNumberQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var page = await bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);

        if (page != null)
        {
            if (page.ContentId.HasValue)
            {
                var file = await fileRepository.GetFileById(page.ContentId.Value, cancellationToken);
                if (file != null)
                {
                    var fc = await fileStorage.GetTextFile(file.FilePath, cancellationToken);
                    page.Text = fc;
                }
            }
            var previousPage = await bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber - 1, cancellationToken);
            var nextPage = await bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber + 1, cancellationToken);

            page.PreviousPage = previousPage;
            page.NextPage = nextPage;
        }

        return page;
    }
}

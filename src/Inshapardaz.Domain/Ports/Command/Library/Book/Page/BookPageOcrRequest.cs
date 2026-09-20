using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Query.File;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class BookPageOcrRequest(int libraryId, int bookId, int sequenceNumber, string apiKey)
    : LibraryBaseCommand(libraryId)
{
    public int BookId { get; set; } = bookId;
    public int SequenceNumber { get; } = sequenceNumber;
    public string ApiKey { get; } = apiKey;
}

public class BookPageOcrRequestHandler(
    IAmACommandProcessor commandProcessor,
    IQueryProcessor queryProcessor,
    IBookPageRepository bookPageRepository,
    IProvideOcr ocr,
    IUserHelper userHelper)
    : RequestHandlerAsync<BookPageOcrRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<BookPageOcrRequest> HandleAsync(BookPageOcrRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var bookPage = await bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
        if (bookPage != null && bookPage.ImageId.HasValue)
        {
            var image = await queryProcessor.ExecuteAsync(new GetFileQuery(bookPage.ImageId.Value, userHelper.AccountId), cancellationToken);

            if (image != null)
            {
                var text = await ocr.PerformOcr(image.Contents, command.ApiKey, cancellationToken);
                bookPage.Text = text;

                var updateBookPageRequest = new UpdateBookPageRequest(command.LibraryId, bookPage.BookId, userHelper.AccountId.Value, bookPage.SequenceNumber, bookPage);
                await commandProcessor.SendAsync(updateBookPageRequest, cancellationToken: cancellationToken);
                
                return await base.HandleAsync(command, cancellationToken);
            }
        }

        throw new NotFoundException();
    }
}

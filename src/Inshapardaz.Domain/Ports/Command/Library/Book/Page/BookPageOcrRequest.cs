using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Query.File;
using Paramore.Brighter;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class BookPageOcrRequest : LibraryBaseCommand
{
    public BookPageOcrRequest(int libraryId, int bookId, int sequenceNumber, string apiKey)
        : base(libraryId)
    {
        BookId = bookId;
        SequenceNumber = sequenceNumber;
        ApiKey = apiKey;
    }

    public int BookId { get; set; }
    public int SequenceNumber { get; }
    public string ApiKey { get; }
}

public class BookPageOcrRequestHandler : RequestHandlerAsync<BookPageOcrRequest>
{
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IProvideOcr _ocr;
    private readonly IUserHelper _userHelper;

    public BookPageOcrRequestHandler(IAmACommandProcessor commandProcessor,
        IQueryProcessor queryProcessor,
        IBookPageRepository bookPageRepository,
        IProvideOcr ocr, 
        IUserHelper userHelper)
    {
        _bookPageRepository = bookPageRepository;
        _queryProcessor = queryProcessor;
        _ocr = ocr;
        _userHelper = userHelper;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<BookPageOcrRequest> HandleAsync(BookPageOcrRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var bookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
        if (bookPage != null && bookPage.ImageId.HasValue)
        {
            var image = await _queryProcessor.ExecuteAsync(new GetFileQuery(bookPage.ImageId.Value), cancellationToken);

            if (image != null)
            {
                var text = await _ocr.PerformOcr(image.Contents, command.ApiKey, cancellationToken);
                bookPage.Text = text;

                var updateBookPageRequest = new UpdateBookPageRequest(command.LibraryId, bookPage.BookId, _userHelper.AccountId.Value, bookPage.SequenceNumber, bookPage);
                await _commandProcessor.SendAsync(updateBookPageRequest, cancellationToken: cancellationToken);
                
                //await _bookPageRepository.UpdatePage(command.LibraryId, bookPage.BookId, bookPage.SequenceNumber, text, bookPage.ImageId.Value, bookPage.Status, bookPage.ChapterId, cancellationToken);
                
                return await base.HandleAsync(command, cancellationToken);
            }
        }

        throw new NotFoundException();
    }
}

using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library.Book;

public class GetBookContentQuery(
    int libraryId,
    int bookId,
    int contentId,
    string language,
    string mimeType,
    int? accountId)
    : LibraryBaseQuery<BookContentModel>(libraryId)
{
    public int BookId { get; set; } = bookId;
    public int ContentId { get; } = contentId;
    public string MimeType { get; set; } = mimeType;
    public int? AccountId { get; } = accountId;
    public string Language { get; set; } = language;
}

public class GetBookContentQueryHandler(
    ILibraryRepository libraryRepository,
    IBookRepository bookRepository,
    IFileRepository fileRepository)
    : QueryHandlerAsync<GetBookContentQuery, BookContentModel>
{
    public override async Task<BookContentModel> ExecuteAsync(GetBookContentQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
        if (book == null)
        {
            throw new NotFoundException();
        }

        if (!book.IsPublic && !command.AccountId.HasValue)
        {
            throw new UnauthorizedException();
        }

        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }

        var bookContent = await bookRepository.GetBookContent(command.LibraryId, command.BookId, command.ContentId, cancellationToken);
        if (bookContent != null)
        {
            if (command.AccountId.HasValue)
            {
                await bookRepository.AddRecentBook(command.LibraryId, command.AccountId.Value, command.BookId, new ReadProgressModel()
                {
                    ProgressType = ProgressType.File,
                    ProgressId = command.ContentId,
                    ProgressValue = 0.0,
                    
                }, cancellationToken);
            }

            if (book.IsPublic)
            {
                bookContent.ContentUrl = await ImageHelper.TryConvertToPublicFile(bookContent.FileId, fileRepository, cancellationToken);
            }
        }

        return bookContent;
    }
}

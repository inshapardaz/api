using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class UpdateBookImageRequest(int libraryId, int bookId, int? accountId) : LibraryBaseCommand(libraryId)
{
    public int BookId { get; } = bookId;
    public int? AccountId { get; } = accountId;
    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateBookImageRequestHandler(
    IBookRepository bookRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<UpdateBookImageRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateBookImageRequest> HandleAsync(UpdateBookImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);

        if (book == null)
        {
            throw new NotFoundException();
        }

        var fileName = FilePathHelper.GetBookImageFileName(command.Image.FileName);
        var filePath = FilePathHelper.GetBookImageFilePath(command.BookId, fileName);

        var saveContentCommand = new SaveFileCommand(fileName, filePath, command.Image.Contents)
        {
            MimeType = command.Image.MimeType,
            ExistingFileId = book.ImageId
        };

        await commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);

        command.Result.File = saveContentCommand.Result;
        if (!book.ImageId.HasValue)
        {
            await bookRepository.UpdateBookImage(command.LibraryId, command.BookId, command.Result.File.Id, cancellationToken);
            command.Result.HasAddedNew = true;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

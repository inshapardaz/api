using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class UpdateBookContentRequest(
    int libraryId,
    int bookId,
    int contentId,
    string language,
    string mimeType,
    int? accountId)
    : LibraryBaseCommand(libraryId)
{
    public int BookId { get; } = bookId;
    public int ContentId { get; } = contentId;
    public string Language { get; } = language;
    public string MimeType { get; } = mimeType;
    public int? AccountId { get; } = accountId;
    public FileModel Content { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public BookContentModel Content { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateBookFileRequestHandler(IBookRepository bookRepository, IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<UpdateBookContentRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]

    public override async Task<UpdateBookContentRequest> HandleAsync(UpdateBookContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
        if (book != null)
        {
            var bookContent = await bookRepository.GetBookContent(command.LibraryId, command.BookId, command.ContentId, cancellationToken);
            long contentId = 0;
            var fileName = FilePathHelper.GetBookContentFileName(command.Content.FileName);
            var filePath = FilePathHelper.GetBookContentPath(command.BookId, fileName);

            var saveFileCommand = new SaveFileCommand(command.Content.FileName, filePath, command.Content.Contents)
            {
                MimeType = command.Content.MimeType,
                ExistingFileId = bookContent?.FileId
            };

            await commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);

            if (bookContent == null)
            {
                contentId = await bookRepository.AddBookContent(command.BookId, saveFileCommand.Result.Id, command.Language, cancellationToken);
                command.Result.HasAddedNew = bookContent is null;
            }
            else
            {
                bookContent.ContentUrl = saveFileCommand.Result.FilePath;
                bookContent.MimeType = command.MimeType;
                bookContent.Language = command.Language;
                
                await bookRepository.UpdateBookContent(command.LibraryId,
                                                        command.BookId,
                                                        command.ContentId,
                                                        command.Language,
                                                        cancellationToken);

                command.Result.Content = bookContent;
                contentId = bookContent.Id;
            }
            
            command.Result.Content = await bookRepository.GetBookContent(command.LibraryId, command.BookId, contentId, cancellationToken); ;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class UpdateBookContentRequest : LibraryBaseCommand
{
    public UpdateBookContentRequest(int libraryId, int bookId, int contentId, string language, string mimeType, int? accountId)
        : base(libraryId)
    {
        BookId = bookId;
        ContentId = contentId;
        Language = language;
        MimeType = mimeType;
        AccountId = accountId;
    }

    public int BookId { get; }
    public int ContentId { get; }
    public string Language { get; }
    public string MimeType { get; }
    public int? AccountId { get; }
    public FileModel Content { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public BookContentModel Content { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateBookFileRequestHandler : RequestHandlerAsync<UpdateBookContentRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public UpdateBookFileRequestHandler(IBookRepository bookRepository, IAmACommandProcessor commandProcessor)
    {
        _bookRepository = bookRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]

    public override async Task<UpdateBookContentRequest> HandleAsync(UpdateBookContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
        if (book != null)
        {
            var bookContent = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, command.ContentId, cancellationToken);
            long contentId = 0;
            var fileName = FilePathHelper.GetBookContentFileName(command.Content.FileName);
            var filePath = FilePathHelper.GetBookContentPath(command.LibraryId, command.BookId, fileName);

            var saveFileCommand = new SaveFileCommand(command.Content.FileName, filePath, command.Content.Contents)
            {
                MimeType = command.Content.MimeType,
                ExistingFileId = bookContent?.FileId
            };

            await _commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);

            if (bookContent == null)
            {
                contentId = await _bookRepository.AddBookContent(command.BookId, saveFileCommand.Result.Id, command.Language, cancellationToken);
                command.Result.HasAddedNew = bookContent is null;
            }
            else
            {
                bookContent.ContentUrl = saveFileCommand.Result.FilePath;
                bookContent.MimeType = command.MimeType;
                bookContent.Language = command.Language;
                
                await _bookRepository.UpdateBookContent(command.LibraryId,
                                                        command.BookId,
                                                        command.ContentId,
                                                        command.Language,
                                                        cancellationToken);

                command.Result.Content = bookContent;
                contentId = bookContent.Id;
            }
            
            command.Result.Content = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, contentId, cancellationToken); ;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

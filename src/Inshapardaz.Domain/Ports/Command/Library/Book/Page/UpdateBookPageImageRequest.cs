using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class UpdateBookPageImageRequest : LibraryBaseCommand
{
    public UpdateBookPageImageRequest(int libraryId, int bookId, int sequenceNumber)
        : base(libraryId)
    {
        BookId = bookId;
        SequenceNumber = sequenceNumber;
    }

    public int BookId { get; }

    public int SequenceNumber { get; }

    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateBookPageImageRequestHandler : RequestHandlerAsync<UpdateBookPageImageRequest>
{
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public UpdateBookPageImageRequestHandler(IBookPageRepository bookPageRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _bookPageRepository = bookPageRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateBookPageImageRequest> HandleAsync(UpdateBookPageImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var bookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);

        if (bookPage == null)
        {
            throw new NotFoundException();
        }

        var fileName = FilePathHelper.GetBookPageFileName(command.Image.FileName);
        var filePath = FilePathHelper.GetBookPageFilePath(command.BookId, fileName);

        var saveContentCommand = new SaveFileCommand(fileName, filePath, command.Image.Contents)
        {
            MimeType = command.Image.MimeType,
            ExistingFileId = bookPage.ImageId
        };

        await _commandProcessor.SendAsync(saveContentCommand, cancellationToken: cancellationToken);
        command.Image = saveContentCommand.Result;

        await _bookPageRepository.UpdatePageImage(command.LibraryId, command.BookId, command.SequenceNumber, saveContentCommand.Result.Id, cancellationToken);
        command.Result.File = saveContentCommand.Result;
        command.Result.HasAddedNew = !bookPage.ImageId.HasValue;

        return await base.HandleAsync(command, cancellationToken);
    }
}

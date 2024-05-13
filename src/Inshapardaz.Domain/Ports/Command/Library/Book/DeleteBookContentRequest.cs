using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class DeleteBookContentRequest : BookRequest
{
    public DeleteBookContentRequest(int libraryId, int bookId, int contentId)
        : base(libraryId, bookId)
    {
        ContentId = contentId;
    }

    public int ContentId { get; }
}

public class DeleteBookContentRequestHandler : RequestHandlerAsync<DeleteBookContentRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public DeleteBookContentRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _bookRepository = bookRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteBookContentRequest> HandleAsync(DeleteBookContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var content = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, command.ContentId, cancellationToken);
        if (content != null)
        {
            await _fileStorage.TryDeleteFile(content.ContentUrl, cancellationToken);
            await _bookRepository.DeleteBookContent(command.LibraryId, command.BookId, command.ContentId, cancellationToken);
            await _fileRepository.DeleteFile(content.FileId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

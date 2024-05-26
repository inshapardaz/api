using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class DeleteBookRequest : BookRequest
{
    public DeleteBookRequest(int libraryId, int bookId, int? accountId)
        : base(libraryId, bookId)
    {
        AccountId = accountId;
    }

    public int? AccountId { get; }
}

public class DeleteBookRequestHandler : RequestHandlerAsync<DeleteBookRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public DeleteBookRequestHandler(IBookRepository bookRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _bookRepository = bookRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteBookRequest> HandleAsync(DeleteBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
        if (book != null)
        {
            if (book.ImageId.HasValue)
            {
                var image = await _fileRepository.GetFileById(book.ImageId.Value, cancellationToken);
                if (image != null && !string.IsNullOrWhiteSpace(image.FilePath))
                {
                    await _fileStorage.TryDeleteImage(image.FilePath, cancellationToken);
                    await _fileRepository.DeleteFile(image.Id, cancellationToken);
                }
            }
            await _bookRepository.DeleteBook(command.LibraryId, command.BookId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

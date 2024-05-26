using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class DeleteBookPageRequest : LibraryBaseCommand
{
    public DeleteBookPageRequest(int libraryId, int bookId, int sequenceNumber)
        : base(libraryId)
    {
        BookId = bookId;
        SequenceNumber = sequenceNumber;
    }

    public int BookId { get; }

    public int SequenceNumber { get; }
}

public class DeleteBookPageRequestHandler : RequestHandlerAsync<DeleteBookPageRequest>
{
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public DeleteBookPageRequestHandler(IBookPageRepository bookPageRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _bookPageRepository = bookPageRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteBookPageRequest> HandleAsync(DeleteBookPageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var bookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);

        if (bookPage != null)
        {
            if (bookPage.ImageId.HasValue)
            {
                var existingImage = await _fileRepository.GetFileById(bookPage.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                await _fileRepository.DeleteFile(existingImage.Id, cancellationToken);
                await _bookPageRepository.DeletePageImage(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
            }

            await _bookPageRepository.DeletePage(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

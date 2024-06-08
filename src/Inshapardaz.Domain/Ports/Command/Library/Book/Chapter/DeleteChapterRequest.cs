using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class DeleteChapterRequest : BookRequest
{
    public DeleteChapterRequest(int libraryId, int bookId, int chapterNumber)
        : base(libraryId, bookId)
    {
        ChapterNumber = chapterNumber;
    }

    public int ChapterNumber { get; }
}

public class DeleteChapterRequestHandler : RequestHandlerAsync<DeleteChapterRequest>
{
    private readonly IChapterRepository _chapterRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStore;

    public DeleteChapterRequestHandler(IChapterRepository chapterRepository,
        IFileRepository fileRepository,
        IFileStorage fileStore)
    {
        _chapterRepository = chapterRepository;
        _fileRepository = fileRepository;
        _fileStore = fileStore;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteChapterRequest> HandleAsync(DeleteChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var chapterContents = await _chapterRepository.GetChapterContents(command.LibraryId,
                    command.BookId, command.ChapterNumber, cancellationToken);
        if (chapterContents != null)
        {
            foreach (var chapterContent in chapterContents)
            {
                //TODO : convert to long
                var file = await _fileRepository.GetFileById((long)chapterContent.FileId, cancellationToken);
                await _fileStore.TryDeleteFile(file.FilePath, cancellationToken);
                await _fileRepository.DeleteFile(file.Id, cancellationToken);
            }
        }

        await _chapterRepository.DeleteChapter(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}

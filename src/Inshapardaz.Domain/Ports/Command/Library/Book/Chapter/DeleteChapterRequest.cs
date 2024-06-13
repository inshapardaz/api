using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
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
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteChapterRequestHandler(IChapterRepository chapterRepository,
        IAmACommandProcessor commandProcessor)
    {
        _chapterRepository = chapterRepository;
        _commandProcessor = commandProcessor;
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
                await _commandProcessor.SendAsync(new DeleteTextFileCommand(chapterContent.FileId), cancellationToken: cancellationToken);
            }
        }

        await _chapterRepository.DeleteChapter(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}

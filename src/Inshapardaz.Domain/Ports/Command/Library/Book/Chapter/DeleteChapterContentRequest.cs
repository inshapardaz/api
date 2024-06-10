using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class DeleteChapterContentRequest : BookRequest
{
    public DeleteChapterContentRequest(int libraryId, int bookId, int chapterNumber, string language)
        : base(libraryId, bookId)
    {
        ChapterNumber = chapterNumber;
        Language = language;
    }

    public int ChapterNumber { get; }

    public string Language { get; }
}

public class DeleteChapterContentRequestHandler : RequestHandlerAsync<DeleteChapterContentRequest>
{
    private readonly IChapterRepository _chapterRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteChapterContentRequestHandler(IChapterRepository chapterRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _chapterRepository = chapterRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteChapterContentRequest> HandleAsync(DeleteChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var chapterContent = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterNumber, command.Language, cancellationToken);
        if (chapterContent != null)
        {
            await _commandProcessor.SendAsync(new DeleteTextFileCommand(chapterContent.FileId), cancellationToken: cancellationToken);
            await _chapterRepository.DeleteChapterContentById(command.LibraryId, command.BookId, command.ChapterNumber, command.Language, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

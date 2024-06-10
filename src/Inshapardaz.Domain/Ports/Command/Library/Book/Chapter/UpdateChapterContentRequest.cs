using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class UpdateChapterContentRequest : BookRequest
{
    public UpdateChapterContentRequest(int libraryId, int bookId, int chapterNumber, string contents, string language)
        : base(libraryId, bookId)
    {
        ChapterNumber = chapterNumber;
        Contents = contents;
        Language = language;
    }

    public string Language { get; set; }

    public string Contents { get; set; }

    public int ChapterNumber { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public ChapterContentModel ChapterContent { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateChapterContentRequestHandler : RequestHandlerAsync<UpdateChapterContentRequest>
{
    private readonly IChapterRepository _chapterRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public UpdateChapterContentRequestHandler(IChapterRepository chapterRepository, 
        ILibraryRepository libraryRepository, 
        IFileStorage fileStorage,
        IFileRepository fileRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _chapterRepository = chapterRepository;
        _libraryRepository = libraryRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateChapterContentRequest> HandleAsync(UpdateChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var chapter = await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);
        if (chapter == null)
        {
            throw new BadRequestException();
        }

        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }

        var content = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterNumber, command.Language, cancellationToken);

        var fileName = FilePathHelper.BookChapterContentFileName;

        var saveFileCommand = new SaveTextFileCommand(fileName, FilePathHelper.GetBookChapterContentPath(command.BookId, fileName), command.Contents)
        {
            MimeType = MimeTypes.Markdown,
            ExistingFileId = content?.FileId
        };

        await _commandProcessor.SendAsync(saveFileCommand, cancellationToken: cancellationToken);

        if (content == null)
        {
            var chapterContent = new ChapterContentModel
            {
                BookId = command.BookId,
                ChapterId = chapter.Id,
                ChapterNumber = command.ChapterNumber,
                Language = command.Language,
                FileId = saveFileCommand.Result.Id
            };

            command.Result.ChapterContent = await _chapterRepository.AddChapterContent(command.LibraryId,
                                                                                      chapterContent,
                                                                                       cancellationToken);
            command.Result.ChapterContent.Text = command.Contents;
            command.Result.HasAddedNew = true;
        }
        else
        {
            await _chapterRepository.UpdateChapterContent(command.LibraryId,
                                                          command.BookId,
                                                          command.ChapterNumber,
                                                          command.Language,
                                                          command.Contents,
                                                          saveFileCommand.Result.Id,
                                                          cancellationToken);
            command.Result.ChapterContent = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterNumber, command.Language, cancellationToken);
            command.Result.ChapterContent.Text = command.Contents;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

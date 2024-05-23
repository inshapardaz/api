using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class AddChapterContentRequest : BookRequest
{
    public AddChapterContentRequest(int libraryId, int bookId, int chapterNumber, string contents, string language)
        : base(libraryId, bookId)
    {
        ChapterNumber = chapterNumber;
        Contents = contents;
        Language = language;
    }

    public int ChapterNumber { get; set; }

    public string Contents { get; }

    public string Language { get; set; }

    public ChapterContentModel Result { get; set; }
}

public class AddChapterContentRequestHandler : RequestHandlerAsync<AddChapterContentRequest>
{
    private readonly IChapterRepository _chapterRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public AddChapterContentRequestHandler(IChapterRepository chapterRepository, ILibraryRepository libraryRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _chapterRepository = chapterRepository;
        _libraryRepository = libraryRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddChapterContentRequest> HandleAsync(AddChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }

        var chapter = await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);
        if (chapter != null)
        {
            var fileName = $"chapter-{chapter.ChapterNumber}.md";
            var url = await StoreFile($"books/{command.BookId}/{fileName}", command.Contents, cancellationToken);
            var file = await AddFile(fileName, url, MimeTypes.Markdown, cancellationToken);

            var chapterContent = new ChapterContentModel
            {
                BookId = command.BookId,
                ChapterId = chapter.Id,
                ChapterNumber = chapter.ChapterNumber,
                Language = command.Language,
                FileId = file?.Id
            };

            command.Result = await _chapterRepository.AddChapterContent(command.LibraryId, chapterContent, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> StoreFile(string filePath, string contents, CancellationToken cancellationToken)
    {
        return await _fileStorage.StoreTextFile(filePath, contents, cancellationToken);
    }

    private async Task<FileModel> AddFile(string fileName, string filePath, string mimeType, CancellationToken cancellationToken)
    {
        return await _fileRepository.AddFile(new FileModel
        {
            FileName = fileName,
            FilePath = filePath,
            MimeType = mimeType,
            DateCreated = DateTime.Now,
            IsPublic = false
        }, cancellationToken);
    }
}

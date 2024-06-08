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
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public UpdateChapterContentRequestHandler(IChapterRepository chapterRepository, ILibraryRepository libraryRepository, IFileStorage fileStorage, IFileRepository fileRepository)
    {
        _chapterRepository = chapterRepository;
        _libraryRepository = libraryRepository;
        _fileStorage = fileStorage;
        _fileRepository = fileRepository;
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

        if (content == null)
        {
            var fileName = $"{Guid.NewGuid().ToString("N")}.md";
            var url = await StoreFile($"books/{command.BookId}/chapters/{fileName}", command.Contents, cancellationToken);
            var file = await AddFile(fileName, url, MimeTypes.Markdown, cancellationToken);
            var chapterContent = new ChapterContentModel
            {
                BookId = command.BookId,
                ChapterId = chapter.Id,
                ChapterNumber = command.ChapterNumber,
                Language = command.Language,
                FileId = file.Id
            };

            command.Result.ChapterContent = await _chapterRepository.AddChapterContent(command.LibraryId,
                                                                                      chapterContent,
                                                                                       cancellationToken);
            command.Result.ChapterContent.Text = command.Contents;
            command.Result.HasAddedNew = true;
        }
        else
        {
            long fileId;
            if (content.FileId.HasValue)
            {
                var file = await _fileRepository.GetFileById(content.FileId.Value, cancellationToken);
                var url = await StoreFile(file.FilePath, command.Contents, cancellationToken);
                fileId = file.Id;
            } 
            else
            {
                var fileName = $"{Guid.NewGuid().ToString("N")}.md";
                var url = await StoreFile($"books/{command.BookId}/chapters/{fileName}", command.Contents, cancellationToken);
                var file = await AddFile(fileName, url, MimeTypes.Markdown, cancellationToken);
                fileId = file.Id;
            }

            await _chapterRepository.UpdateChapterContent(command.LibraryId,
                                                          command.BookId,
                                                          command.ChapterNumber,
                                                          command.Language,
                                                          command.Contents,
                                                          fileId,
                                                          cancellationToken);
            command.Result.ChapterContent = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterNumber, command.Language, cancellationToken);
            command.Result.ChapterContent.Text = command.Contents;
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

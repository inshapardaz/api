using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateChapterContentRequest : BookRequest
    {
        public UpdateChapterContentRequest(int libraryId, int bookId, int chapterId, string contents, string language, string mimetype)
            : base(libraryId, bookId)
        {
            ChapterId = chapterId;
            Contents = contents;
            MimeType = mimetype;
            Language = language;
        }

        public string MimeType { get; set; }

        public string Language { get; set; }
        public string Contents { get; set; }

        public int ChapterId { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public ChapterContentModel ChapterContent { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateChapterContentRequestHandler : RequestHandlerAsync<UpdateChapterContentRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IFileRepository _fileRepository;
        private readonly ILibraryRepository _libraryRepository;

        public UpdateChapterContentRequestHandler(IBookRepository bookRepository, IChapterRepository chapterRepository, IFileStorage fileStorage,
                                                  IFileRepository fileRepository, ILibraryRepository libraryRepository)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
            _fileStorage = fileStorage;
            _fileRepository = fileRepository;
            _libraryRepository = libraryRepository;
        }

        public override async Task<UpdateChapterContentRequest> HandleAsync(UpdateChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);

            if (book == null)
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

            var contentUrl = await _chapterRepository.GetChapterContentUrl(command.LibraryId, command.BookId, command.ChapterId, command.Language, command.MimeType, cancellationToken);

            if (contentUrl == null)
            {
                var name = GenerateChapterContentUrl(command.BookId, command.ChapterId, command.Language, command.MimeType);
                var actualUrl = await _fileStorage.StoreTextFile(name, command.Contents, cancellationToken);

                var fileModel = new Models.FileModel { MimeType = command.MimeType, FilePath = actualUrl, IsPublic = book.IsPublic, FileName = name };
                var file = await _fileRepository.AddFile(fileModel, cancellationToken);
                var chapterContent = new ChapterContentModel
                {
                    BookId = command.BookId,
                    ChapterId = command.ChapterId,
                    Language = command.Language,
                    MimeType = command.MimeType,
                    FileId = file.Id
                };

                command.Result.ChapterContent = await _chapterRepository.AddChapterContent(command.LibraryId,
                                                                                           chapterContent,
                                                                                           cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                string url = contentUrl ?? GenerateChapterContentUrl(command.BookId, command.ChapterId, command.Language, command.MimeType);
                var actualUrl = await _fileStorage.StoreTextFile(url, command.Contents, cancellationToken);

                await _chapterRepository.UpdateChapterContent(command.LibraryId,
                                                              command.BookId,
                                                              command.ChapterId,
                                                              command.Language,
                                                              command.MimeType,
                                                              actualUrl,
                                                              cancellationToken);
                command.Result.ChapterContent = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterId, command.Language, command.MimeType, cancellationToken);
            }

            command.Result.ChapterContent.ContentUrl = await ImageHelper.TryConvertToPublicFile(command.Result.ChapterContent.FileId, _fileRepository, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }

        private string GenerateChapterContentUrl(int bookId, int chapterId, string language, string mimeType)
        {
            var extension = MimetypeToExtension(mimeType);
            return $"books/{bookId}/chapters/{chapterId}_{language}.{extension}";
        }

        private string MimetypeToExtension(string mimeType)
        {
            switch (mimeType.ToLower())
            {
                case "text/plain": return "txt";
                case "text/markdown": return "md";
                case "text/html": return "md";
                case "application/msword": return "doc";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": return "doc";
                case "application/pdf": return "pdf";
                case "application/epub+zip": return "epub";
                default: throw new BadRequestException();
            }
        }
    }
}

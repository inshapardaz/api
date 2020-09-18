using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddChapterContentRequest : BookRequest
    {
        public AddChapterContentRequest(ClaimsPrincipal claims, int libraryId, int bookId, int chapterId, string contents, string language, string mimeType, Guid? userId)
            : base(claims, libraryId, bookId, userId)
        {
            ChapterId = chapterId;
            Contents = contents;
            MimeType = mimeType;
            Language = language;
        }

        public int ChapterId { get; set; }

        public string Contents { get; }

        public string MimeType { get; set; }

        public string Language { get; set; }

        public ChapterContentModel Result { get; set; }
    }

    public class AddChapterContentRequestHandler : RequestHandlerAsync<AddChapterContentRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IFileStorage _fileStorage;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IFileRepository _fileRepository;

        public AddChapterContentRequestHandler(IBookRepository bookRepository, IChapterRepository chapterRepository, IFileStorage fileStorage, ILibraryRepository libraryRepository, IFileRepository fileRepository)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
            _fileStorage = fileStorage;
            _libraryRepository = libraryRepository;
            _fileRepository = fileRepository;
        }

        [Authorise(step: 0, HandlerTiming.Before, Permission.Admin, Permission.LibraryAdmin, Permission.Writer)]
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

            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);

            var chapter = await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterId, cancellationToken);
            if (chapter != null)
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

                command.Result = await _chapterRepository.AddChapterContent(command.LibraryId, chapterContent, cancellationToken);

                if (file.IsPublic)
                {
                    var url = await ImageHelper.TryConvertToPublicFile(file.Id, _fileRepository, cancellationToken);
                    command.Result.ContentUrl = url;
                }
            }

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
                case "application/msword": return "docx";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": return "docx";
                case "application/pdf": return "pdf";
                case "application/epub+zip": return "epub";
                default: throw new BadRequestException();
            }
        }
    }
}

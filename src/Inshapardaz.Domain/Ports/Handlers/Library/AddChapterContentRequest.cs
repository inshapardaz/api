using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddChapterContentRequest : BookRequest
    {
        public AddChapterContentRequest(ClaimsPrincipal claims, int libraryId, int bookId, int chapterId, string contents, string mimeType, Guid userId)
            : base(claims, libraryId, bookId, userId)
        {
            ChapterId = chapterId;
            Contents = contents;
            MimeType = mimeType;
        }

        public int ChapterId { get; set; }

        public string Contents { get; }

        public string MimeType { get; set; }

        public ChapterContentModel Result { get; set; }
    }

    public class AddChapterContentRequestHandler : RequestHandlerAsync<AddChapterContentRequest>
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IFileStorage _fileStorage;

        public AddChapterContentRequestHandler(IChapterRepository chapterRepository, IFileStorage fileStorage)
        {
            _chapterRepository = chapterRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddChapterContentRequest> HandleAsync(AddChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapter = await _chapterRepository.GetChapterById(command.ChapterId, cancellationToken);
            if (chapter != null)
            {
                var name = GenerateChapterContentUrl(command.BookId, command.ChapterId, command.MimeType);
                var actualUrl = await _fileStorage.StoreTextFile(name, command.Contents, cancellationToken);

                command.Result = await _chapterRepository.AddChapterContent(command.BookId, command.ChapterId, command.MimeType, actualUrl, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private string GenerateChapterContentUrl(int bookId, int chapterId, string mimeType)
        {
            var extension = MimetypeToExtension(mimeType);
            return $"books/{bookId}/chapters/{chapterId}.{extension}";
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

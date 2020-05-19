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
    public class UpdateChapterContentRequest : BookRequest
    {
        public UpdateChapterContentRequest(ClaimsPrincipal claims, int libraryId, int bookId, int chapterId, string contents, string mimetype, Guid userId)
            : base(claims, libraryId, bookId, userId)
        {
            ChapterId = chapterId;
            Contents = contents;
            MimeType = mimetype;
        }

        public string MimeType { get; set; }

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
        private readonly IChapterRepository _chapterRepository;
        private readonly IFileStorage _fileStorage;

        public UpdateChapterContentRequestHandler(IChapterRepository chapterRepository, IFileStorage fileStorage)
        {
            _chapterRepository = chapterRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<UpdateChapterContentRequest> HandleAsync(UpdateChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var contentUrl = await _chapterRepository.GetChapterContentUrl(command.LibraryId, command.BookId, command.ChapterId, command.MimeType, cancellationToken);

            if (contentUrl == null)
            {
                var name = GenerateChapterContentUrl(command.BookId, command.ChapterId, command.MimeType);
                var actualUrl = await _fileStorage.StoreTextFile(name, command.Contents, cancellationToken);

                command.Result.ChapterContent = await _chapterRepository.AddChapterContent(command.LibraryId,
                                                                                           command.BookId,
                                                                                           command.ChapterId,
                                                                                           command.MimeType,
                                                                                           actualUrl,
                                                                                           cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                string url = contentUrl ?? GenerateChapterContentUrl(command.BookId, command.ChapterId, command.MimeType);
                var actualUrl = await _fileStorage.StoreTextFile(url, command.Contents, cancellationToken);

                await _chapterRepository.UpdateChapterContent(command.LibraryId,
                                                              command.BookId,
                                                              command.ChapterId,
                                                              command.MimeType,
                                                              actualUrl,
                                                              cancellationToken);
                command.Result.ChapterContent = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterId, command.MimeType, cancellationToken);
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

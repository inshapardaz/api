using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetChapterContentQuery : LibraryBaseQuery<ChapterContentModel>
    {
        public GetChapterContentQuery(int libraryId, int bookId, int chapterId, string mimeType, Guid userId)
            : base(libraryId)
        {
            UserId = userId;
            BookId = bookId;
            ChapterId = chapterId;
            MimeType = mimeType;
        }

        public int BookId { get; set; }

        public int ChapterId { get; }

        public Guid UserId { get; set; }

        public string MimeType { get; set; }
    }

    public class GetChapterContentQueryHandler : QueryHandlerAsync<GetChapterContentQuery, ChapterContentModel>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IFileStorage _fileStorage;

        public GetChapterContentQueryHandler(IBookRepository bookRepository, IChapterRepository chapterRepository, IFileStorage fileStorage)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<ChapterContentModel> ExecuteAsync(GetChapterContentQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapterContent = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterId, command.MimeType, cancellationToken);
            if (chapterContent != null)
            {
                chapterContent.Content = await _fileStorage.GetTextFile(chapterContent.ContentUrl, cancellationToken);
                if (command.UserId != Guid.Empty)
                {
                    await _bookRepository.AddRecentBook(command.LibraryId, command.UserId, command.BookId, cancellationToken);
                }
            }

            return chapterContent;
        }
    }
}

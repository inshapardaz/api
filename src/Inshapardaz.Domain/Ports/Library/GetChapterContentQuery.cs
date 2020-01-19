using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetChapterContentQuery : IQuery<ChapterContent>
    {
        public GetChapterContentQuery(int bookId, int chapterId, string mimeType, Guid userId)
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

    public class GetChapterContentQueryHandler : QueryHandlerAsync<GetChapterContentQuery, ChapterContent>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;

        public GetChapterContentQueryHandler(IBookRepository bookRepository, IChapterRepository chapterRepository)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
        }

        public override async Task<ChapterContent> ExecuteAsync(GetChapterContentQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapterContent = await _chapterRepository.GetChapterContent(command.BookId, command.ChapterId, command.MimeType, cancellationToken);
            if (chapterContent != null)
            {
                if (command.UserId != Guid.Empty)
                {
                    await _bookRepository.AddRecentBook(command.UserId, command.BookId, cancellationToken);
                }
            }

            return chapterContent;
        }
    }
}


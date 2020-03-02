using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetChapterByIdQuery : IQuery<ChapterModel>
    {
        public GetChapterByIdQuery(int bookId, int chapterId, Guid userId)
        {
            UserId = userId;
            BookId = bookId;
            ChapterId = chapterId;
        }

        public int BookId { get; set; }

        public int ChapterId { get; }

        public Guid UserId { get; set; }
    }

    public class GetChapterByIdQueryHandler : QueryHandlerAsync<GetChapterByIdQuery, ChapterModel>
    {
        private readonly IChapterRepository _chapterRepository;

        public GetChapterByIdQueryHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public override async Task<ChapterModel> ExecuteAsync(GetChapterByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapter = await _chapterRepository.GetChapterById(command.ChapterId, cancellationToken);

            if (chapter != null)
            {
                chapter.Contents = await _chapterRepository.GetChapterContents(command.BookId, command.ChapterId, cancellationToken);
            }

            return chapter;
        }
    }
}

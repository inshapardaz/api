using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetChapterByIdQuery : LibraryBaseQuery<ChapterModel>
    {
        public GetChapterByIdQuery(int libraryId, int bookId, int chapterId, Guid? userId)
            : base(libraryId)
        {
            UserId = userId;
            BookId = bookId;
            ChapterId = chapterId;
        }

        public int BookId { get; set; }

        public int ChapterId { get; }

        public Guid? UserId { get; set; }
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
            return await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterId, cancellationToken);
        }
    }
}

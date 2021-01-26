using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetChapterByIdQuery : LibraryBaseQuery<ChapterModel>
    {
        public GetChapterByIdQuery(int libraryId, int bookId, int chapterNumber)
            : base(libraryId)
        {
            BookId = bookId;
            ChapterNumber = chapterNumber;
        }

        public int BookId { get; set; }

        public int ChapterNumber { get; }
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
            return await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);
        }
    }
}

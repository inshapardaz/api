using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteChapterContentRequest : BookRequest
    {
        public DeleteChapterContentRequest(int bookId, int chapterId, int contentId)
            : base(bookId)
        {
            ChapterId = chapterId;
            ContentId = contentId;
        }

        public int ChapterId { get; }

        public int ContentId { get; }
    }

    public class DeleteChapterContentRequestHandler : RequestHandlerAsync<DeleteChapterContentRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public DeleteChapterContentRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public override async Task<DeleteChapterContentRequest> HandleAsync(DeleteChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _chapterRepository.DeleteChapterContentById(command.BookId, command.ChapterId, command.ContentId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

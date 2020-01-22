using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteChapterRequest : BookRequest
    {
        public DeleteChapterRequest(int bookId, int chapterId, Guid userId)
            : base(bookId, userId)
        {
            ChapterId = chapterId;
        }

        public int ChapterId { get; }
    }

    public class DeleteChapterRequestHandler : RequestHandlerAsync<DeleteChapterRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public DeleteChapterRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public override async Task<DeleteChapterRequest> HandleAsync(DeleteChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _chapterRepository.DeleteChapter(command.ChapterId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

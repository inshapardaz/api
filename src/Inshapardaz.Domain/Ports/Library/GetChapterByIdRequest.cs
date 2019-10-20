using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetChapterByIdRequest : BookRequest
    {
        public GetChapterByIdRequest(int bookId, int chapterId, Guid userId)
            : base(bookId, userId)
        {
            ChapterId = chapterId;
        }

        public Chapter Result { get; set; }
        public int ChapterId { get; }
    }

    public class GetChapterByIdRequestHandler : RequestHandlerAsync<GetChapterByIdRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public GetChapterByIdRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        [BookRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetChapterByIdRequest> HandleAsync(GetChapterByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapter = await _chapterRepository.GetChapterById(command.ChapterId, cancellationToken);

            if (chapter != null)
            {
                chapter.Contents = await _chapterRepository.GetChapterContents(command.BookId, command.ChapterId, cancellationToken);
                command.Result = chapter;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class CheckChapterContentRequest : BookRequest
    {
        public CheckChapterContentRequest(int bookId, int chapterId)
            : base(bookId)
        {
            ChapterId = chapterId;
        }

        public int ChapterId { get; }

        public Guid UserId { get; set; }

        public bool Result { get; set; }
    }

    public class CheckChapterContentRequestHandler : RequestHandlerAsync<CheckChapterContentRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public CheckChapterContentRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public override async Task<CheckChapterContentRequest> HandleAsync(CheckChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapters = await _chapterRepository.HasChapterContents(command.BookId, command.ChapterId, cancellationToken);
            command.Result = chapters;
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}


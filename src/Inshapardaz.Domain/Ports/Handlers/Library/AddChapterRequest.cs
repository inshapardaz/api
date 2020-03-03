using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddChapterRequest : BookRequest
    {
        public AddChapterRequest(ClaimsPrincipal claims, int libraryId, int bookId, ChapterModel chapter, Guid userId)
            : base(claims, libraryId, bookId, userId)
        {
            Chapter = chapter;
        }

        public ChapterModel Chapter { get; }

        public ChapterModel Result { get; set; }
    }

    public class AddChapterRequestHandler : RequestHandlerAsync<AddChapterRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public AddChapterRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddChapterRequest> HandleAsync(AddChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _chapterRepository.AddChapter(command.BookId, command.Chapter, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddChapterContentRequest : BookRequest
    {
        public AddChapterContentRequest(ChapterContent contents)
            : base(contents.BookId)
        {
            Contents = contents;
        }

        public ChapterContent Contents { get; }
    }

    public class AddChapterContentRequestHandler : RequestHandlerAsync<AddChapterContentRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public AddChapterContentRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        [BookRequestValidation(1, HandlerTiming.Before)]
        public override async Task<AddChapterContentRequest> HandleAsync(AddChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapter = await _chapterRepository.GetChapterById(command.Contents.ChapterId, cancellationToken);
            if (chapter == null)
            {
                throw new BadRequestException();
            }

            await _chapterRepository.AddChapterContent(command.Contents, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}

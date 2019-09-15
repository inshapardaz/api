using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddChapterRequest : BookRequest
    {
        public AddChapterRequest(int bookId, Chapter chapter, Guid userId)
            : base(bookId, userId)
        {
            Chapter = chapter;
        }

        public Chapter Chapter { get; }

        public Chapter Result { get; set; }
    }

    public class AddChapterRequestHandler : RequestHandlerAsync<AddChapterRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public AddChapterRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        [BookRequestValidation(1, HandlerTiming.Before)]
        public override async Task<AddChapterRequest> HandleAsync(AddChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result= await _chapterRepository.AddChapter(command.BookId, command.Chapter, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}

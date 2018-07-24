using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetChaptersByBookRequest : BookRequest
    {
        public GetChaptersByBookRequest(int bookId)
            : base(bookId)
        {
        }

        public IEnumerable<Chapter> Result { get; set; }

        public Guid UserId { get; set; }
    }

    public class GetChaptersByBookRequestHandler : RequestHandlerAsync<GetChaptersByBookRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public GetChaptersByBookRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public override async Task<GetChaptersByBookRequest> HandleAsync(GetChaptersByBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapters = await _chapterRepository.GetChaptersByBook(command.BookId, cancellationToken);

            foreach (var chapter in chapters)
            {
                chapter.HasContents = await _chapterRepository.HasChapterContents(chapter.BookId, chapter.Id, cancellationToken);
            }

            command.Result = chapters;
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}


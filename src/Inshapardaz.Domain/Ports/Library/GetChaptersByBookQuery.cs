using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetChaptersByBookQuery : IQuery<IEnumerable<Chapter>>
    {
        public GetChaptersByBookQuery(int bookId, Guid userId)
        {
            UserId = userId;
            BookId = bookId;
        }

        public int BookId { get; set; }


        public Guid UserId { get; set; }
    }

    public class GetChaptersByBookQuerytHandler : QueryHandlerAsync<GetChaptersByBookQuery, IEnumerable<Chapter>>
    {
        private readonly IChapterRepository _chapterRepository;

        public GetChaptersByBookQuerytHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        [BookRequestValidation(1, HandlerTiming.Before)]
        public override async Task<IEnumerable<Chapter>> ExecuteAsync(GetChaptersByBookQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapters = await _chapterRepository.GetChaptersByBook(command.BookId, cancellationToken);

            if (!chapters.Any()) return null;

            foreach (var chapter in chapters)
            {
                var contents = await _chapterRepository.GetChapterContents(command.BookId, chapter.Id, cancellationToken);
                chapter.Contents = contents;
            }

            return chapters;
        }
    }
}


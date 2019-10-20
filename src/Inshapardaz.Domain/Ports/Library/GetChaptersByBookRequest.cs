﻿using System;
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
        public GetChaptersByBookRequest(int bookId, Guid userId)
            : base(bookId, userId)
        {
        }

        public IEnumerable<Chapter> Result { get; set; }
    }

    public class GetChaptersByBookRequestHandler : RequestHandlerAsync<GetChaptersByBookRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public GetChaptersByBookRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        [BookRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetChaptersByBookRequest> HandleAsync(GetChaptersByBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapters = await _chapterRepository.GetChaptersByBook(command.BookId, cancellationToken);

            foreach (var chapter in chapters)
            {
                var contents = await _chapterRepository.GetChapterContents(command.BookId, chapter.Id, cancellationToken);
                chapter.Contents = contents;
            }

            command.Result = chapters;
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}


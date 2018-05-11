﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsByTitlesQueryHandler : QueryHandlerAsync<GetWordsByTitlesQuery, IEnumerable<Word>>
    {
        private readonly IWordRepository _wordRepository;

        public GetWordsByTitlesQueryHandler(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }


        public override async Task<IEnumerable<Word>> ExecuteAsync(GetWordsByTitlesQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _wordRepository.GetWordsByTitles(query.DictionaryId, query.Titles, cancellationToken);
        }
    }
}

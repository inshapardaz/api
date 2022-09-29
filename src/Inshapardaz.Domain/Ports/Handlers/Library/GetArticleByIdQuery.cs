﻿using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetArticleByIdQuery : LibraryBaseQuery<ArticleModel>
    {
        public GetArticleByIdQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            SequenceNumber = sequenceNumber;
        }

        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public int SequenceNumber { get; }
    }

    public class GetArticleByIdQueryHandler : QueryHandlerAsync<GetArticleByIdQuery, ArticleModel>
    {
        private readonly IArticleRepository _articleRepository;

        public GetArticleByIdQueryHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public override async Task<ArticleModel> ExecuteAsync(GetArticleByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var article = await _articleRepository.GetArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

            if (article != null)
            {
                article.PreviousArticle = await _articleRepository.GetArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber - 1, cancellationToken);
                article.NextArticle = await _articleRepository.GetArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber + 1, cancellationToken);
            }

            return article;
        }
    }
}

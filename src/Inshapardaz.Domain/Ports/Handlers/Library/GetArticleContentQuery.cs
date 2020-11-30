using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories.Library;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetArticleContentQuery : LibraryBaseQuery<ArticleContentModel>
    {
        public GetArticleContentQuery(int libraryId, int periodicalId, int issueId, int articleId, string language, string mimeType, int? userId)
            : base(libraryId)
        {
            MimeType = mimeType;
            UserId = userId;
            PeriodicalId = periodicalId;
            IssueId = issueId;
            ArticleId = articleId;
            Language = language;
        }

        public string MimeType { get; set; }
        public int? UserId { get; }
        public int PeriodicalId { get; }
        public int IssueId { get; }
        public int ArticleId { get; }
        public string Language { get; set; }
    }

    public class GetArticleContentQueryHandler : QueryHandlerAsync<GetArticleContentQuery, ArticleContentModel>
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IFileRepository _fileRepository;

        public GetArticleContentQueryHandler(ILibraryRepository libraryRepository, IArticleRepository articleRepository, IIssueRepository issueRepository, IFileRepository fileRepository)
        {
            _libraryRepository = libraryRepository;
            _articleRepository = articleRepository;
            _issueRepository = issueRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<ArticleContentModel> ExecuteAsync(GetArticleContentQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _articleRepository.GetArticleContentById(command.LibraryId, command.PeriodicalId, command.IssueId, command.ArticleId, command.Language, command.MimeType, cancellationToken);
        }
    }
}

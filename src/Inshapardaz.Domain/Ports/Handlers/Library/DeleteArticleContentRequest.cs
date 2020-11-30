using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteArticleContentRequest : LibraryAuthorisedCommand
    {
        public DeleteArticleContentRequest(ClaimsPrincipal claims, int libraryId, int periodicalId, int issueId, int articleId, string language, string mimeType, int? userId)
            : base(claims, libraryId)
        {
            MimeType = mimeType;
            PeriodicalId = periodicalId;
            IssueId = issueId;
            ArticleId = articleId;
            Language = language;
        }

        public string MimeType { get; }
        public int PeriodicalId { get; }
        public int IssueId { get; }
        public int ArticleId { get; }
        public string Language { get; }
    }

    public class DeleteArticleContentRequestHandler : RequestHandlerAsync<DeleteArticleContentRequest>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteArticleContentRequestHandler(IArticleRepository chapterRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _articleRepository = chapterRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<DeleteArticleContentRequest> HandleAsync(DeleteArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var content = await _articleRepository.GetArticleContent(command.LibraryId, command.PeriodicalId, command.IssueId, command.ArticleId, command.Language, command.MimeType, cancellationToken);

            if (content != null)
            {
                if (!string.IsNullOrWhiteSpace(content.ContentUrl))
                {
                    await _fileStorage.TryDeleteFile(content.ContentUrl, cancellationToken);
                }

                await _fileRepository.DeleteFile(content.FileId, cancellationToken);
                await _articleRepository.DeleteArticleContentById(command.LibraryId, command.PeriodicalId, command.IssueId, command.ArticleId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue.Article
{
    public class UpdateIssueArticleContentRequest : LibraryBaseCommand
    {
        public UpdateIssueArticleContentRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string content, string language)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            SequenceNumber = sequenceNumber;
            Content = content;
            Language = language;
        }

        public string Language { get; set; }
        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public int SequenceNumber { get; }
        public string Content { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public ArticleContentModel Content { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateArticleContentRequestHandler : RequestHandlerAsync<UpdateIssueArticleContentRequest>
    {
        private readonly IIssueArticleRepository _articleRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IFileRepository _fileRepository;
        private readonly ILibraryRepository _libraryRepository;

        public UpdateArticleContentRequestHandler(IIssueArticleRepository articleRepository, IIssueRepository issueRepository, IFileStorage fileStorage,
                                                  IFileRepository fileRepository, ILibraryRepository libraryRepository)
        {
            _articleRepository = articleRepository;
            _issueRepository = issueRepository;
            _fileStorage = fileStorage;
            _fileRepository = fileRepository;
            _libraryRepository = libraryRepository;
        }

        public override async Task<UpdateIssueArticleContentRequest> HandleAsync(UpdateIssueArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
            var article = await _articleRepository.GetArticle(command.LibraryId, command.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, command.SequenceNumber, cancellationToken);

            if (issue == null || article == null)
            {
                throw new BadRequestException();
            }

            if (string.IsNullOrWhiteSpace(command.Language))
            {
                var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
                if (library == null)
                {
                    throw new BadRequestException();
                }

                command.Language = library.Language;
            }

            var contentUrl = await _articleRepository.GetArticleContentById(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Language, cancellationToken);

            if (contentUrl == null)
            {
                command.Result.Content = await _articleRepository.AddArticleContent(
                    command.LibraryId,
                    command.PeriodicalId,
                    command.VolumeNumber,
                    command.IssueNumber,
                    command.SequenceNumber,
                    command.Language,
                    command.Content,
                    cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                command.Result.Content = await _articleRepository.UpdateArticleContent(command.LibraryId,
                                                        command.PeriodicalId,
                                                        command.VolumeNumber,
                                                        command.IssueNumber,
                                                        command.SequenceNumber,
                                                        command.Language,
                                                        command.Content,
                                                        cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

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
    public class AddIssueArticleContentRequest : LibraryBaseCommand
    {
        public AddIssueArticleContentRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string contents, string language)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            SequenceNumber = sequenceNumber;
            Content = contents;
            Language = language;
        }

        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public int SequenceNumber { get; }
        public string Content { get; }

        public string Language { get; set; }

        public ArticleContentModel Result { get; set; }
    }

    public class AddArticleContentRequestHandler : RequestHandlerAsync<AddIssueArticleContentRequest>
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IIssueArticleRepository _articleRepository;
        private readonly IFileStorage _fileStorage;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IFileRepository _fileRepository;

        public AddArticleContentRequestHandler(IIssueRepository issueRepository, IIssueArticleRepository chapterRepository, IFileStorage fileStorage, ILibraryRepository libraryRepository, IFileRepository fileRepository)
        {
            _issueRepository = issueRepository;
            _articleRepository = chapterRepository;
            _fileStorage = fileStorage;
            _libraryRepository = libraryRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<AddIssueArticleContentRequest> HandleAsync(AddIssueArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Language))
            {
                var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
                if (library == null)
                {
                    throw new BadRequestException();
                }

                command.Language = library.Language;
            }

            var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
            if (issue == null)
            {
                throw new BadRequestException();
            }

            var article = await _articleRepository.GetArticle(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

            if (article != null)
            {
                command.Result = await _articleRepository.AddArticleContent(
                    command.LibraryId,
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

using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Paramore.Brighter;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue.Page
{
    public class IssuePageOcrRequest : LibraryBaseCommand
    {
        public IssuePageOcrRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string apiKey)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            SequenceNumber = sequenceNumber;
            ApiKey = apiKey;
        }
        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public int SequenceNumber { get; }
        public string ApiKey { get; }
    }

    public class IssuePageOcrRequestHandler : RequestHandlerAsync<IssuePageOcrRequest>
    {
        private readonly IIssuePageRepository _issuePageRepository;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IProvideOcr _ocr;

        public IssuePageOcrRequestHandler(IIssuePageRepository issuePageRepository, IQueryProcessor queryProcessor, IProvideOcr ocr)
        {
            _issuePageRepository = issuePageRepository;
            _queryProcessor = queryProcessor;
            _ocr = ocr;
        }

        public override async Task<IssuePageOcrRequest> HandleAsync(IssuePageOcrRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issuePage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
            if (issuePage != null && issuePage.ImageId.HasValue)
            {
                var image = await _queryProcessor.ExecuteAsync(new GetFileQuery(issuePage.ImageId.Value, 0, 0));

                if (image != null)
                {
                    var text = await _ocr.PerformOcr(image.Contents, command.ApiKey, cancellationToken);
                    issuePage.Text = text;
                    await _issuePageRepository.UpdatePage(command.LibraryId, issuePage.PeriodicalId, issuePage.VolumeNumber, issuePage.IssueNumber, issuePage.SequenceNumber, text, issuePage.ImageId.Value, issuePage.ArticleNumber, issuePage.Status, cancellationToken);
                    return await base.HandleAsync(command, cancellationToken);
                }
            }

            throw new NotFoundException();
        }
    }
}

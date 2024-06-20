using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Query.File;
using Paramore.Brighter;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

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
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IProvideOcr _ocr;

    public IssuePageOcrRequestHandler(IIssuePageRepository issuePageRepository, 
        IQueryProcessor queryProcessor, 
        IAmACommandProcessor commandProcessor,
        IProvideOcr ocr)
    {
        _issuePageRepository = issuePageRepository;
        _queryProcessor = queryProcessor;
        _ocr = ocr;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<IssuePageOcrRequest> HandleAsync(IssuePageOcrRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issuePage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        if (issuePage != null && issuePage.ImageId.HasValue)
        {
            var image = await _queryProcessor.ExecuteAsync(new GetFileQuery(issuePage.ImageId.Value));

            if (image != null)
            {
                issuePage.Text = await _ocr.PerformOcr(image.Contents, command.ApiKey, cancellationToken);

                var cmd = new UpdateIssuePageRequest(command.LibraryId, issuePage);
                await _commandProcessor.SendAsync(cmd, cancellationToken: cancellationToken);
                return await base.HandleAsync(command, cancellationToken);
            }
        }

        throw new NotFoundException();
    }
}

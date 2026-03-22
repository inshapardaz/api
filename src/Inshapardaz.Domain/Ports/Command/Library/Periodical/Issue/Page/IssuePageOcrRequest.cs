using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Query.File;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Page;

public class IssuePageOcrRequest(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int sequenceNumber,
    string apiKey)
    : LibraryBaseCommand(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int SequenceNumber { get; } = sequenceNumber;
    public string ApiKey { get; } = apiKey;
}

public class IssuePageOcrRequestHandler(
    IIssuePageRepository issuePageRepository,
    IQueryProcessor queryProcessor,
    IAmACommandProcessor commandProcessor,
    IProvideOcr ocr)
    : RequestHandlerAsync<IssuePageOcrRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<IssuePageOcrRequest> HandleAsync(IssuePageOcrRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var issuePage = await issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
        if (issuePage != null && issuePage.ImageId.HasValue)
        {
            var image = await queryProcessor.ExecuteAsync(new GetFileQuery(issuePage.ImageId.Value));

            if (image != null)
            {
                issuePage.Text = await ocr.PerformOcr(image.Contents, command.ApiKey, cancellationToken);

                var cmd = new UpdateIssuePageRequest(command.LibraryId, issuePage);
                await commandProcessor.SendAsync(cmd, cancellationToken: cancellationToken);
                return await base.HandleAsync(command, cancellationToken);
            }
        }

        throw new NotFoundException();
    }
}

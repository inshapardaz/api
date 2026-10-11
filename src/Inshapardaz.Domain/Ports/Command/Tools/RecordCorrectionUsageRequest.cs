using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools;

// Editors apply corrections while typing and report how often each one fired, in batches.
public class RecordCorrectionUsageRequest(string language, string profile, IEnumerable<CorrectionUsageModel> usages) : RequestBase
{
    public const int MaxEntries = 1000;
    public const int MaxCountPerEntry = 10000;

    public string Language { get; } = language;
    public string Profile { get; } = profile;
    public IEnumerable<CorrectionUsageModel> Usages { get; } = usages;
}

public class RecordCorrectionUsageRequestHandler(ICorrectionRepository correctionRepository)
    : RequestHandlerAsync<RecordCorrectionUsageRequest>
{
    [AuthorizeEditor(1)]
    public override async Task<RecordCorrectionUsageRequest> HandleAsync(RecordCorrectionUsageRequest command, CancellationToken cancellationToken = default)
    {
        CorrectionValidation.ValidateProfile(command.Profile);

        var usages = (command.Usages ?? [])
            .Where(u => u.Count > 0)
            .GroupBy(u => u.Id)
            .Select(g => new CorrectionUsageModel { Id = g.Key, Count = Math.Min(g.Sum(u => u.Count), RecordCorrectionUsageRequest.MaxCountPerEntry) })
            .ToList();

        if (usages.Count > RecordCorrectionUsageRequest.MaxEntries)
        {
            throw new BadRequestException($"At most {RecordCorrectionUsageRequest.MaxEntries} corrections can be reported at once.");
        }

        if (usages.Count > 0)
        {
            await correctionRepository.IncrementUsage(command.Language, command.Profile, usages, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

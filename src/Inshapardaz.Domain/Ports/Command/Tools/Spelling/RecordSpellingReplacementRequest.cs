using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers.Spelling;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.Spelling;

// Records that a writer replaced a misspelt word with a suggestion, so the same replacement is
// ranked higher next time.
public class RecordSpellingReplacementRequest(string language, string word, string replacement) : RequestBase
{
    public string Language { get; } = language;
    public string Word { get; } = word;
    public string Replacement { get; } = replacement;
}

public class RecordSpellingReplacementRequestHandler(ISpellingReplacementRepository replacementRepository)
    : RequestHandlerAsync<RecordSpellingReplacementRequest>
{
    [AuthorizeEditor(1)]
    public override async Task<RecordSpellingReplacementRequest> HandleAsync(RecordSpellingReplacementRequest command, CancellationToken cancellationToken = default)
    {
        var source = WordNormaliser.Normalise(command.Word);
        var replacement = command.Replacement?.Trim();
        if (source.Length == 0 || string.IsNullOrEmpty(replacement))
        {
            throw new BadRequestException("Both the word and its replacement are required.");
        }

        await replacementRepository.RecordReplacement(command.Language, source, replacement, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}

using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools;

// Loads many corrections into one list. A correction whose incorrect text is already in the list
// replaces that entry's correct text and complete-word flag (keeping its usage); blank entries
// and repeats within the upload are skipped.
public class ImportCorrectionsRequest(string language, string profile, IEnumerable<CorrectionModel> corrections) : RequestBase
{
    public string Language { get; } = language;
    public string Profile { get; } = profile;
    public IEnumerable<CorrectionModel> Corrections { get; } = corrections;
    public CorrectionImportResult Result { get; set; }

    public class CorrectionImportResult
    {
        public int Added { get; set; }
        public int Updated { get; set; }
        public int Skipped { get; set; }
    }
}

public class ImportCorrectionsRequestHandler(ICorrectionRepository correctionRepository)
    : RequestHandlerAsync<ImportCorrectionsRequest>
{
    [AuthorizeLanguageTools(1)]
    public override async Task<ImportCorrectionsRequest> HandleAsync(ImportCorrectionsRequest command, CancellationToken cancellationToken = default)
    {
        CorrectionValidation.ValidateProfile(command.Profile);

        var existing = (await correctionRepository.GetAllCorrections(command.Language, command.Profile, cancellationToken))
            .GroupBy(c => c.IncorrectText, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var result = new ImportCorrectionsRequest.CorrectionImportResult();

        foreach (var correction in command.Corrections ?? [])
        {
            if (string.IsNullOrEmpty(correction?.IncorrectText) || string.IsNullOrEmpty(correction.CorrectText) || !seen.Add(correction.IncorrectText))
            {
                result.Skipped++;
                continue;
            }

            if (existing.TryGetValue(correction.IncorrectText, out var current))
            {
                if (current.CorrectText == correction.CorrectText && current.CompleteWord == correction.CompleteWord)
                {
                    result.Skipped++;
                    continue;
                }

                current.CorrectText = correction.CorrectText;
                current.CompleteWord = correction.CompleteWord;
                await correctionRepository.UpdateCorrection(current, cancellationToken);
                result.Updated++;
            }
            else
            {
                await correctionRepository.AddCorrection(new CorrectionModel
                {
                    Language = command.Language,
                    Profile = command.Profile,
                    IncorrectText = correction.IncorrectText,
                    CorrectText = correction.CorrectText,
                    CompleteWord = correction.CompleteWord
                }, cancellationToken);
                result.Added++;
            }
        }

        command.Result = result;
        return await base.HandleAsync(command, cancellationToken);
    }
}

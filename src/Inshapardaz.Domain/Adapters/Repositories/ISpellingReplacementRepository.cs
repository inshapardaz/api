using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Adapters.Repositories;

// History of the replacements writers picked for misspelt words, used to rank suggestions.
// Source words are stored normalised (see WordNormaliser).
public interface ISpellingReplacementRepository
{
    Task<IEnumerable<SpellingReplacementModel>> GetReplacements(string language, IEnumerable<string> normalisedSourceWords, CancellationToken cancellationToken);

    Task RecordReplacement(string language, string normalisedSourceWord, string replacement, CancellationToken cancellationToken);
}

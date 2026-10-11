using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers.Dictionaries;
using Inshapardaz.Domain.Models.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.Dictionaries;

// Folds several articles (typically numbered homographs such as "bank 1", "bank 2") into the first
// one: senses are concatenated, links de-duplicated, raw definitions joined, and the rest deleted.
public class MergeDictionaryArticlesRequest(long dictionaryId, IEnumerable<long> articleIds, string headword) : RequestBase
{
    public long DictionaryId { get; } = dictionaryId;
    public IEnumerable<long> ArticleIds { get; } = articleIds;

    // Headword for the merged article; defaults to the first article's without its number suffix.
    public string Headword { get; } = headword;

    public DictionaryArticleModel Result { get; set; }
}

public class MergeDictionaryArticlesRequestHandler(IDictionaryRepository dictionaryRepository, IUserHelper userHelper)
    : RequestHandlerAsync<MergeDictionaryArticlesRequest>
{
    [Authorize(1)]
    public override async Task<MergeDictionaryArticlesRequest> HandleAsync(MergeDictionaryArticlesRequest command, CancellationToken cancellationToken = default)
    {
        var dictionary = await dictionaryRepository.GetDictionaryById(command.DictionaryId, cancellationToken)
            ?? throw new NotFoundException();
        DictionaryPermissions.EnsureCanWrite(userHelper, dictionary);

        var ids = (command.ArticleIds ?? []).Distinct().ToList();
        if (ids.Count < 2)
        {
            throw new BadRequestException("At least two articles are needed to merge.");
        }

        var articles = new List<DictionaryArticleModel>();
        foreach (var id in ids)
        {
            articles.Add(await dictionaryRepository.GetArticleById(command.DictionaryId, id, cancellationToken)
                ?? throw new BadRequestException($"Article {id} is not in this dictionary."));
        }

        var primary = articles[0];
        primary.Headword = string.IsNullOrWhiteSpace(command.Headword)
            ? HeadwordGrouping.BaseHeadword(primary.Headword)
            : command.Headword.Trim();
        primary.Pronunciation = articles.Select(a => a.Pronunciation).FirstOrDefault(p => !string.IsNullOrWhiteSpace(p));
        primary.Senses = articles.SelectMany(a => a.Senses).ToList();
        primary.Links = articles.SelectMany(a => a.Links)
            .GroupBy(l => (l.LinkType, (l.TargetWord ?? string.Empty).ToLowerInvariant()))
            .Select(g => g.First())
            .ToList();
        var raw = string.Join("<hr>", articles.Select(a => a.RawDefinition).Where(r => !string.IsNullOrWhiteSpace(r)));
        primary.RawDefinition = raw.Length == 0 ? null : raw;

        command.Result = await dictionaryRepository.UpdateArticle(primary, cancellationToken);
        await dictionaryRepository.DeleteArticles(command.DictionaryId, articles.Skip(1).Select(a => a.Id), cancellationToken);
        await dictionaryRepository.UpdateEntryCount(command.DictionaryId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}

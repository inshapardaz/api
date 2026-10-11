using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers.Dictionaries;
using Inshapardaz.Domain.Models.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.Dictionaries;

public class AddDictionaryArticleRequest(DictionaryArticleModel article) : RequestBase
{
    public DictionaryArticleModel Article { get; } = article;
    public DictionaryArticleModel Result { get; set; }
}

public class AddDictionaryArticleRequestHandler(IDictionaryRepository dictionaryRepository, IUserHelper userHelper)
    : RequestHandlerAsync<AddDictionaryArticleRequest>
{
    [Authorize(1)]
    public override async Task<AddDictionaryArticleRequest> HandleAsync(AddDictionaryArticleRequest command, CancellationToken cancellationToken = default)
    {
        var dictionary = await dictionaryRepository.GetDictionaryById(command.Article.DictionaryId, cancellationToken)
            ?? throw new NotFoundException();
        DictionaryPermissions.EnsureCanWrite(userHelper, dictionary);
        DictionaryArticleValidation.Validate(command.Article);

        command.Result = await dictionaryRepository.AddArticle(command.Article, cancellationToken);
        await dictionaryRepository.UpdateEntryCount(dictionary.Id, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}

public class UpdateDictionaryArticleRequest(DictionaryArticleModel article) : RequestBase
{
    public DictionaryArticleModel Article { get; } = article;
    public DictionaryArticleModel Result { get; set; }
}

public class UpdateDictionaryArticleRequestHandler(IDictionaryRepository dictionaryRepository, IUserHelper userHelper)
    : RequestHandlerAsync<UpdateDictionaryArticleRequest>
{
    [Authorize(1)]
    public override async Task<UpdateDictionaryArticleRequest> HandleAsync(UpdateDictionaryArticleRequest command, CancellationToken cancellationToken = default)
    {
        var dictionary = await dictionaryRepository.GetDictionaryById(command.Article.DictionaryId, cancellationToken)
            ?? throw new NotFoundException();
        DictionaryPermissions.EnsureCanWrite(userHelper, dictionary);
        DictionaryArticleValidation.Validate(command.Article);

        _ = await dictionaryRepository.GetArticleById(command.Article.DictionaryId, command.Article.Id, cancellationToken)
            ?? throw new NotFoundException();
        command.Result = await dictionaryRepository.UpdateArticle(command.Article, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}

public class DeleteDictionaryArticleRequest(long dictionaryId, long articleId) : RequestBase
{
    public long DictionaryId { get; } = dictionaryId;
    public long ArticleId { get; } = articleId;
}

public class DeleteDictionaryArticleRequestHandler(IDictionaryRepository dictionaryRepository, IUserHelper userHelper)
    : RequestHandlerAsync<DeleteDictionaryArticleRequest>
{
    [Authorize(1)]
    public override async Task<DeleteDictionaryArticleRequest> HandleAsync(DeleteDictionaryArticleRequest command, CancellationToken cancellationToken = default)
    {
        var dictionary = await dictionaryRepository.GetDictionaryById(command.DictionaryId, cancellationToken);
        if (dictionary != null)
        {
            DictionaryPermissions.EnsureCanWrite(userHelper, dictionary);
            await dictionaryRepository.DeleteArticles(command.DictionaryId, [command.ArticleId], cancellationToken);
            await dictionaryRepository.UpdateEntryCount(command.DictionaryId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}

internal static class DictionaryArticleValidation
{
    public static void Validate(DictionaryArticleModel article)
    {
        if (string.IsNullOrWhiteSpace(article.Headword))
        {
            throw new BadRequestException("An article needs a headword.");
        }

        if (article.Headword.Trim().Length > 500)
        {
            throw new BadRequestException("A headword can be at most 500 characters.");
        }
    }
}

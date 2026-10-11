using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Helpers.Dictionaries;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Api.Converters;

public interface IRenderDictionary
{
    PageView<DictionaryView> Render(PageRendererArgs<DictionaryModel> source);
    DictionaryView Render(DictionaryModel dictionary);
    PageView<DictionaryArticleView> RenderArticles(PageRendererArgs<DictionaryArticleModel> source, DictionaryModel dictionary);
    DictionaryArticleView RenderArticle(DictionaryArticleModel article, DictionaryModel dictionary);

    // Articles gathered from several dictionaries (lookups, merge candidates): self links only.
    IEnumerable<DictionaryArticleView> RenderArticles(IEnumerable<DictionaryArticleModel> articles);
}

public class DictionaryRenderer(IRenderLink linkRenderer, IUserHelper userHelper) : IRenderDictionary
{
    public PageView<DictionaryView> Render(PageRendererArgs<DictionaryModel> source)
    {
        var page = new PageView<DictionaryView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
        {
            Data = source.Page.Data?.Select(Render).ToList()
        };

        page.Links = PageLinks(page, source.RouteArguments.Query, nameof(DictionaryController.GetDictionaries), new { });

        if (userHelper.CanManageLanguageTools)
        {
            page.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(DictionaryController.AddDictionary),
                Method = HttpMethod.Post,
                Rel = RelTypes.Create
            }));
            page.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(DictionaryController.ImportDictionary),
                Method = HttpMethod.Post,
                Rel = RelTypes.BulkUpload
            }));
        }

        return page;
    }

    public DictionaryView Render(DictionaryModel dictionary)
    {
        var view = dictionary.Map();
        var parameters = new { id = dictionary.Id };

        view.Links.Add(linkRenderer.Render(new Link
        {
            ActionName = nameof(DictionaryController.GetDictionaryById),
            Method = HttpMethod.Get,
            Rel = RelTypes.Self,
            Parameters = parameters
        }));
        view.Links.Add(linkRenderer.Render(new Link
        {
            ActionName = nameof(DictionaryController.GetDictionaryArticles),
            Method = HttpMethod.Get,
            Rel = RelTypes.Articles,
            Parameters = parameters
        }));
        view.Links.Add(linkRenderer.Render(new Link
        {
            ActionName = nameof(DictionaryController.ExportDictionary),
            Method = HttpMethod.Get,
            Rel = RelTypes.Download,
            Parameters = parameters
        }));

        if (DictionaryPermissions.CanWrite(userHelper, dictionary))
        {
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(DictionaryController.UpdateDictionary),
                Method = HttpMethod.Put,
                Rel = RelTypes.Update,
                Parameters = parameters
            }));
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(DictionaryController.DeleteDictionary),
                Method = HttpMethod.Delete,
                Rel = RelTypes.Delete,
                Parameters = parameters
            }));
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(DictionaryController.AddDictionaryArticle),
                Method = HttpMethod.Post,
                Rel = RelTypes.CreateArticle,
                Parameters = parameters
            }));
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(DictionaryController.GetMergeCandidates),
                Method = HttpMethod.Get,
                Rel = RelTypes.MergeCandidates,
                Parameters = parameters
            }));
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(DictionaryController.MergeDictionaryArticles),
                Method = HttpMethod.Post,
                Rel = RelTypes.Merge,
                Parameters = parameters
            }));
        }

        return view;
    }

    public PageView<DictionaryArticleView> RenderArticles(PageRendererArgs<DictionaryArticleModel> source, DictionaryModel dictionary)
    {
        var page = new PageView<DictionaryArticleView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
        {
            Data = source.Page.Data?.Select(a => RenderArticle(a, dictionary)).ToList()
        };

        page.Links = PageLinks(page, source.RouteArguments.Query, nameof(DictionaryController.GetDictionaryArticles), new { id = dictionary.Id });

        if (DictionaryPermissions.CanWrite(userHelper, dictionary))
        {
            page.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(DictionaryController.AddDictionaryArticle),
                Method = HttpMethod.Post,
                Rel = RelTypes.Create,
                Parameters = new { id = dictionary.Id }
            }));
        }

        return page;
    }

    public DictionaryArticleView RenderArticle(DictionaryArticleModel article, DictionaryModel dictionary)
    {
        var view = article.Map();
        var parameters = new { id = article.DictionaryId, articleId = article.Id };

        view.Links.Add(linkRenderer.Render(new Link
        {
            ActionName = nameof(DictionaryController.GetDictionaryArticleById),
            Method = HttpMethod.Get,
            Rel = RelTypes.Self,
            Parameters = parameters
        }));

        if (dictionary != null && DictionaryPermissions.CanWrite(userHelper, dictionary))
        {
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(DictionaryController.UpdateDictionaryArticle),
                Method = HttpMethod.Put,
                Rel = RelTypes.Update,
                Parameters = parameters
            }));
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(DictionaryController.DeleteDictionaryArticle),
                Method = HttpMethod.Delete,
                Rel = RelTypes.Delete,
                Parameters = parameters
            }));
        }

        return view;
    }

    public IEnumerable<DictionaryArticleView> RenderArticles(IEnumerable<DictionaryArticleModel> articles) =>
        articles.Select(a => RenderArticle(a, null)).ToList();

    private List<LinkView> PageLinks<T>(PageView<T> page, string query, string actionName, object parameters)
    {
        Dictionary<string, string> QueryString(int pageNumber) => new()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", page.PageSize.ToString() },
            { "query", query }
        };

        var links = new List<LinkView>
        {
            linkRenderer.Render(new Link
            {
                ActionName = actionName,
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = parameters,
                QueryString = QueryString(page.CurrentPageIndex)
            })
        };

        if (page.CurrentPageIndex < page.PageCount)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = actionName,
                Method = HttpMethod.Get,
                Rel = RelTypes.Next,
                Parameters = parameters,
                QueryString = QueryString(page.CurrentPageIndex + 1)
            }));
        }

        if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = actionName,
                Method = HttpMethod.Get,
                Rel = RelTypes.Previous,
                Parameters = parameters,
                QueryString = QueryString(page.CurrentPageIndex - 1)
            }));
        }

        return links;
    }
}

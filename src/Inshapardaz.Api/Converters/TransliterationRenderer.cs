using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Transliteration;

namespace Inshapardaz.Api.Converters;

public interface IRenderTransliteration
{
    PageView<TransliterationEntryView> Render(PageRendererArgs<TransliterationEntryModel> source, string language);
    TransliterationEntryView Render(TransliterationEntryModel entry);
    TransliterationResultView Render(TransliterationResultModel result);
}

public class TransliterationRenderer(IRenderLink linkRenderer, IUserHelper userHelper) : IRenderTransliteration
{
    public PageView<TransliterationEntryView> Render(PageRendererArgs<TransliterationEntryModel> source, string language)
    {
        var page = new PageView<TransliterationEntryView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
        {
            Data = source.Page.Data?.Select(Render).ToList()
        };

        Dictionary<string, string> QueryString(int pageNumber) => new()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", page.PageSize.ToString() },
            { "query", source.RouteArguments.Query }
        };

        var links = new List<LinkView>
        {
            linkRenderer.Render(new Link
            {
                ActionName = nameof(TransliterationController.GetTransliterationEntries),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { language },
                QueryString = QueryString(page.CurrentPageIndex)
            })
        };

        if (userHelper.CanManageLanguageTools)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(TransliterationController.AddTransliterationEntry),
                Method = HttpMethod.Post,
                Rel = RelTypes.Create,
                Parameters = new { language }
            }));
        }

        if (page.CurrentPageIndex < page.PageCount)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(TransliterationController.GetTransliterationEntries),
                Method = HttpMethod.Get,
                Rel = RelTypes.Next,
                Parameters = new { language },
                QueryString = QueryString(page.CurrentPageIndex + 1)
            }));
        }

        if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(TransliterationController.GetTransliterationEntries),
                Method = HttpMethod.Get,
                Rel = RelTypes.Previous,
                Parameters = new { language },
                QueryString = QueryString(page.CurrentPageIndex - 1)
            }));
        }

        page.Links = links;
        return page;
    }

    public TransliterationEntryView Render(TransliterationEntryModel entry)
    {
        var view = new TransliterationEntryView
        {
            Id = entry.Id,
            Language = entry.Language,
            Roman = entry.Roman,
            Native = entry.Native,
            Meaning = entry.Meaning,
            Category = entry.Category
        };
        var parameters = new { language = entry.Language, id = entry.Id };

        view.Links.Add(linkRenderer.Render(new Link
        {
            ActionName = nameof(TransliterationController.GetTransliterationEntryById),
            Method = HttpMethod.Get,
            Rel = RelTypes.Self,
            Parameters = parameters
        }));

        if (userHelper.CanManageLanguageTools)
        {
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(TransliterationController.UpdateTransliterationEntry),
                Method = HttpMethod.Put,
                Rel = RelTypes.Update,
                Parameters = parameters
            }));
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(TransliterationController.DeleteTransliterationEntry),
                Method = HttpMethod.Delete,
                Rel = RelTypes.Delete,
                Parameters = parameters
            }));
        }

        return view;
    }

    public TransliterationResultView Render(TransliterationResultModel result) => new()
    {
        Result = result.Result,
        From = result.From,
        To = result.To,
        Words = result.Words?.Select(w => new TransliteratedWordView { Input = w.Input, Output = w.Output, Source = w.Source, EntryId = w.EntryId }).ToList()
    };
}

using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Converters;

public interface IRenderCorrection
{
    PageView<CorrectionView> Render(PageRendererArgs<CorrectionModel> source, string language, string profile, CorrectionSortByType sortBy = CorrectionSortByType.IncorrectText);
    IEnumerable<CorrectionSimpleView> RenderSimple(IEnumerable<CorrectionModel> source);

    CorrectionView Render(CorrectionModel correction);
}

public class CorrectionRenderer(IRenderLink linkRenderer, IUserHelper userHelper) : IRenderCorrection
{
    public PageView<CorrectionView> Render(PageRendererArgs<CorrectionModel> source, string language, string profile, CorrectionSortByType sortBy = CorrectionSortByType.IncorrectText)
    {
        var page = new PageView<CorrectionView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
        {
            Data = source.Page.Data?.Select(x => Render(x))
        };

        var links = new List<LinkView>
        {
            linkRenderer.Render(new Link {
                ActionName = nameof(ToolController.GetCorrections),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { language = language, profile = profile },
                QueryString = new Dictionary<string, string>()
                {
                    { "pageNumber" , page.CurrentPageIndex.ToString() },
                    { "pageSize", page.PageSize.ToString() },
                    { "query", source.RouteArguments.Query },
                    { "sortBy", sortBy == CorrectionSortByType.Usage ? "usage" : null }
                }
            })
        };

        if (userHelper.IsEditor)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(ToolController.AddCorrection),
                Method = HttpMethod.Post,
                Rel = RelTypes.Create,
                Parameters = new { language = language, profile = profile }
            }));
        }

        if (page.CurrentPageIndex < page.PageCount)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(ToolController.GetCorrections),
                Method = HttpMethod.Get,
                Rel = RelTypes.Next,
                Parameters = new { language = language, profile = profile },
                QueryString = new Dictionary<string, string>()
                {
                    { "pageNumber" , (page.CurrentPageIndex + 1).ToString() },
                    { "pageSize", page.PageSize.ToString() },
                    { "query", source.RouteArguments.Query },
                    { "sortBy", sortBy == CorrectionSortByType.Usage ? "usage" : null }
                }
            }));
        }

        if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(ToolController.GetCorrections),
                Method = HttpMethod.Get,
                Rel = RelTypes.Previous,
                Parameters = new { language = language, profile = profile },
                QueryString = new Dictionary<string, string>()
                {
                    { "pageNumber" , (page.CurrentPageIndex - 1).ToString() },
                    { "pageSize", page.PageSize.ToString() },
                    { "query", source.RouteArguments.Query },
                    { "sortBy", sortBy == CorrectionSortByType.Usage ? "usage" : null }
                }
            }));
        }

        page.Links = links;
        return page;
    }

    public CorrectionView Render(CorrectionModel correction)
    {
        var view = correction.Map();

        view.Links.Add(linkRenderer.Render(new Link
        {
            ActionName = nameof(ToolController.GetCorrectionById),
            Method = HttpMethod.Get,
            Rel = RelTypes.Self,
            Parameters = new { language = correction.Language, profile = correction.Profile, id = correction.Id }
        }));


        if (userHelper.IsEditor)
        {
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(ToolController.UpdateCorrection),
                Method = HttpMethod.Put,
                Rel = RelTypes.Update,
                Parameters = new { language = correction.Language, profile = correction.Profile, id = correction.Id }
            }));

            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(ToolController.DeleteCorrection),
                Method = HttpMethod.Delete,
                Rel = RelTypes.Delete,
                Parameters = new { language = correction.Language, profile = correction.Profile, id = correction.Id }
            }));
        }

        return view;
    }

    public IEnumerable<CorrectionSimpleView> RenderSimple(IEnumerable<CorrectionModel> source) => source.Select(x => x.MapSimple());
}

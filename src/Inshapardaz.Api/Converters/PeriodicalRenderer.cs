using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Converters;

public interface IRenderPeriodical
{
    PageView<PeriodicalView> Render(PageRendererArgs<PeriodicalModel, PeriodicalFilter, PeriodicalSortByType> source, int libraryId);

    PeriodicalView Render(PeriodicalModel source, int libraryId);
}

public class PeriodicalRenderer(
    IRenderLink linkRenderer,
    IUserHelper userHelper,
    IRenderCategory categoryRenderer,
    IFileStorage fileStorage)
    : IRenderPeriodical
{
    public PageView<PeriodicalView> Render(PageRendererArgs<PeriodicalModel, PeriodicalFilter, PeriodicalSortByType> source, int libraryId)
    {
        var page = new PageView<PeriodicalView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
        {
            Data = source.Page.Data?.Select(x => Render(x, libraryId))
        };

        var pageQuery = CreateQueryString(source, page, page.CurrentPageIndex);

        var links = new List<LinkView>
        {
            linkRenderer.Render(new Link {
                ActionName = nameof(PeriodicalController.GetPeriodicals),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId },
                QueryString = pageQuery
            })
        };

        if (userHelper.IsWriter(libraryId) || userHelper.IsAdmin || userHelper.IsLibraryAdmin(libraryId))
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(PeriodicalController.CreatePeriodical),
                Method = HttpMethod.Post,
                Rel = RelTypes.Create,
                Parameters = new { libraryId = libraryId }
            }));
        }

        if (page.CurrentPageIndex < page.PageCount)
        {
            var nextPageQuery = CreateQueryString(source, page, page.CurrentPageIndex + 1);

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(PeriodicalController.GetPeriodicals),
                Method = HttpMethod.Get,
                Rel = RelTypes.Next,
                Parameters = new { libraryId = libraryId },
                QueryString = nextPageQuery
            }));
        }

        if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
        {
            var prevPageQuery = CreateQueryString(source, page, page.CurrentPageIndex - 1);

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(PeriodicalController.GetPeriodicals),
                Method = HttpMethod.Get,
                Rel = RelTypes.Previous,
                Parameters = new { libraryId = libraryId },
                QueryString = prevPageQuery
            }));
        }

        page.Links = links;
        return page;
    }

    public PeriodicalView Render(PeriodicalModel source, int libraryId)
    {
        var result = source.Map();

        var links = new List<LinkView>
        {
            linkRenderer.Render(new Link
            {
                ActionName = nameof(PeriodicalController.GetPeriodicalById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId, periodicalId = source.Id }
            }),
            linkRenderer.Render(new Link
            {
                ActionName = nameof(IssueController.GetIssues),
                Method = HttpMethod.Get,
                Rel = RelTypes.Issues,
                Parameters = new { libraryId = libraryId, periodicalId = source.Id },
                QueryString = new Dictionary<string, string>
                {
                    { "authorid", source.Id.ToString() }
                }
            })
        };

        if (!string.IsNullOrWhiteSpace(source.ImageUrl) && fileStorage.SupportsPublicLink)
        {
            links.Add(new LinkView
            {
                Href = fileStorage.GetPublicUrl(source.ImageUrl),
                Method = "GET",
                Rel = RelTypes.Image,
                Accept = MimeTypes.Jpg
            });
        }
        else if (source.ImageId.HasValue)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(FileController.GetLibraryFile),
                Method = HttpMethod.Get,
                Rel = RelTypes.Image,
                Parameters = new { libraryId = libraryId, fileId = source.ImageId.Value }
            }));
        }

        if (source.Categories != null)
        {
            var categories = new List<CategoryView>();
            foreach (var category in source.Categories)
            {
                categories.Add(categoryRenderer.Render(category, source.LibraryId));
            }

            result.Categories = categories;
        }

        if (userHelper.IsWriter(libraryId) || userHelper.IsAdmin || userHelper.IsLibraryAdmin(libraryId))
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(PeriodicalController.UpdatePeriodical),
                Method = HttpMethod.Put,
                Rel = RelTypes.Update,
                Parameters = new { libraryId = libraryId, periodicalId = source.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(PeriodicalController.DeletePeriodical),
                Method = HttpMethod.Delete,
                Rel = RelTypes.Delete,
                Parameters = new { libraryId = libraryId, periodicalId = source.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(IssueController.CreateIssue),
                Method = HttpMethod.Post,
                Rel = RelTypes.CreateIssue,
                Parameters = new { libraryId = libraryId, periodicalId = source.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(PeriodicalController.UpdatePeriodicalImage),
                Method = HttpMethod.Put,
                Rel = RelTypes.ImageUpload,
                Parameters = new { libraryId = libraryId, periodicalId = source.Id }
            }));
        }

        result.Links = links;
        return result;
    }

    private static Dictionary<string, string> CreateQueryString(PageRendererArgs<PeriodicalModel, PeriodicalFilter, PeriodicalSortByType> source, PageView<PeriodicalView> page, int pageNumber)
    {
        Dictionary<string, string> queryString = new Dictionary<string, string> {
                { "pageSize", page.PageSize.ToString() }
            };

        if (pageNumber > 1)
        {
            queryString.Add("pageNumber", (pageNumber).ToString());
        }

        if (!string.IsNullOrWhiteSpace(source.RouteArguments.Query))
        {
            queryString.Add("query", source.RouteArguments.Query);
        }

        if (source.Filters != null)
        {
            if (source.Filters.Frequency.HasValue)
                queryString.Add("frequency", source.Filters.Frequency.Value.ToDescription());

            if (source.Filters.CategoryId.HasValue)
                queryString.Add("category", source.Filters.CategoryId.Value.ToString());
            
            if (source.Filters.TagId.HasValue)
                queryString.Add("tag", source.Filters.TagId.Value.ToString());
        }

        if (source.RouteArguments.SortBy != PeriodicalSortByType.Title)
            queryString.Add("sortby", source.RouteArguments.SortBy.ToDescription());

        if (source.RouteArguments.SortDirection != SortDirection.Ascending)
            queryString.Add("sortDirection", source.RouteArguments.SortDirection.ToDescription());
        return queryString;
    }
}

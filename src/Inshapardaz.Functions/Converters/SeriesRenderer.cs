using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Files;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Inshapardaz.Functions.Converters
{
    public static class SeriesRenderer
    {
        public static PageView<SeriesView> Render(this PageRendererArgs<SeriesModel> source, int libraryId, ClaimsPrincipal principal)
        {
            var page = new PageView<SeriesView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => x.Render(libraryId, principal))
            };

            var links = new List<LinkView>
            {
                source.LinkFuncWithParameter(libraryId, page.CurrentPageIndex, page.PageSize, source.RouteArguments.Query, RelTypes.Self)
            };

            if (principal.IsWriter())
            {
                links.Add(AddSeries.Link(libraryId, RelTypes.Create));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(source.LinkFuncWithParameter(libraryId, page.CurrentPageIndex + 1, page.PageSize, source.RouteArguments.Query, RelTypes.Next));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                links.Add(source.LinkFuncWithParameter(libraryId, page.CurrentPageIndex - 1, page.PageSize, source.RouteArguments.Query, RelTypes.Previous));
            }

            page.Links = links;
            return page;
        }

        public static SeriesView Render(this SeriesModel series, int libraryId, ClaimsPrincipal principal)
        {
            var view = series.Map();

            view.Links.Add(GetSeriesById.Link(libraryId, series.Id, RelTypes.Self));
            view.Links.Add(GetBooksBySeries.Link(libraryId, series.Id, RelTypes.Books));

            if (!string.IsNullOrWhiteSpace(series.ImageUrl))
            {
                view.Links.Add(new LinkView { Href = series.ImageUrl, Method = "GET", Rel = RelTypes.Image, Accept = MimeTypes.Jpg });
            }
            else if (series.ImageId.HasValue)
            {
                view.Links.Add(GetFileById.Link(series.ImageId.Value, RelTypes.Image));
            }

            if (principal.IsWriter())
            {
                view.Links.Add(UpdateSeriesImage.Link(libraryId, series.Id, RelTypes.ImageUpload));
                view.Links.Add(UpdateSeries.Link(libraryId, series.Id, RelTypes.Update));
                view.Links.Add(DeleteSeries.Link(libraryId, series.Id, RelTypes.Delete));
            }

            return view;
        }
    }
}

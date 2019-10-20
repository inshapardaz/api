using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Converters
{
    public static class SeriesRenderer
    {
        public static ListView<SeriesView> Render(this IEnumerable<Series> seriesList, ClaimsPrincipal principal)
        {
            var items = seriesList.Select(g => g.Render(principal));
            var view = new ListView<SeriesView> { Items = items };
            view.Links.Add(GetSeries.Link(RelTypes.Self));

            if (principal.IsWriter())
            {
                view.Links.Add(AddSeries.Link(RelTypes.Create));
            }

            return view;
        }

        public static SeriesView Render(this Series series, ClaimsPrincipal principal)
        {
            var view = series.Map();

            view.Links.Add(GetSeriesById.Link(series.Id, RelTypes.Self));
            view.Links.Add(GetBooksBySeries.Link(series.Id, RelTypes.Books));

            if (principal.IsWriter())
            {
                // TODO: Add image upload for series
                //view.Links.Add(UpdateSeriesImage.Link(series.Id, RelTypes.ImageUpload));
                view.Links.Add(UpdateSeries.Link(series.Id, RelTypes.Update));
                view.Links.Add(DeleteSeries.Link(series.Id, RelTypes.Delete));
            }

            return view;
        }
    }
}

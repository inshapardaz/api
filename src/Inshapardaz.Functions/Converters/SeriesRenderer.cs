using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Books;
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
        public static ListView<SeriesView> Render(this IEnumerable<SeriesModel> seriesList, int libraryId, ClaimsPrincipal principal)
        {
            var items = seriesList.Select(g => g.Render(libraryId, principal));
            var view = new ListView<SeriesView> { Items = items };
            view.Links.Add(GetSeries.Link(libraryId));

            if (principal.IsWriter())
            {
                view.Links.Add(AddSeries.Link(libraryId, RelTypes.Create));
            }

            return view;
        }

        public static SeriesView Render(this SeriesModel series, int libraryId, ClaimsPrincipal principal)
        {
            var view = series.Map();

            view.Links.Add(GetSeriesById.Link(libraryId, series.Id, RelTypes.Self));
            view.Links.Add(GetBooksBySeries.Link(libraryId, series.Id, RelTypes.Books));

            if (principal.IsWriter())
            {
                // TODO: Add image upload for series
                //view.Links.Add(UpdateSeriesImage.Link(series.Id, RelTypes.ImageUpload));
                view.Links.Add(UpdateSeries.Link(libraryId, series.Id, RelTypes.Update));
                view.Links.Add(DeleteSeries.Link(libraryId, series.Id, RelTypes.Delete));
            }

            return view;
        }
    }
}

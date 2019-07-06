using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Adapters.Library
{
    public interface IRenderSeries
    {
        SeriesView Render(ClaimsPrincipal principal, Series series);
    }

    public class SeriesRenderer : IRenderSeries
    {
        public SeriesView Render(ClaimsPrincipal principal, Series series)
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

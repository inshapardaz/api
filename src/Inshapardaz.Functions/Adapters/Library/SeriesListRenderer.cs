using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.View.Library;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Adapters.Library
{
    public interface IRenderSeriesList
    {
        ListView<SeriesView> Render(ClaimsPrincipal principal, IEnumerable<Series> seriesList);
    }

    public class SeriesListRenderer : IRenderSeriesList
    {
        private readonly IRenderSeries _seriesRenderer;

        public SeriesListRenderer(IRenderSeries seriesRenderer)
        {
            _seriesRenderer = seriesRenderer;
        }

        public ListView<SeriesView> Render(ClaimsPrincipal principal, IEnumerable<Series> seriesList)
        {
            var items = seriesList.Select(g => _seriesRenderer.Render(principal, g));
            var view = new ListView<SeriesView> { Items = items };
            view.Links.Add(GetSeries.Link(RelTypes.Self));

            if (principal.IsWriter())
            {
                view.Links.Add(AddSeries.Link(RelTypes.Create));
            }

            return view;
        }
    }
}

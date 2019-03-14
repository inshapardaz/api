using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderSeriesList
    {
        ListView<SeriesView> RenderResult(IEnumerable<Series> seriesList);
    }

    public class SeriesListRenderer : IRenderSeriesList
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IRenderSeries _seriesRenderer;

        public SeriesListRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IRenderSeries seriesRenderer)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _seriesRenderer = seriesRenderer;
        }

        public ListView<SeriesView> RenderResult(IEnumerable<Series> seriesList)
        {
            var items = seriesList.Select(g => _seriesRenderer.RenderResult(g));
            var view = new ListView<SeriesView> { Items = items };
            view.Links.Add(_linkRenderer.Render("GetSeries", RelTypes.Self));

            if (_userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render("CreateSeries", RelTypes.Create));
            }

            return view;
        }
    }
}

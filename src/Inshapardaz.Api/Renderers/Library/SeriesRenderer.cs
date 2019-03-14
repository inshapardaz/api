using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderSeries
    {
        SeriesView RenderResult(Series series);
    }

    public class SeriesRenderer : IRenderSeries
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public SeriesRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }
        public SeriesView RenderResult(Series series)
        {
            var view = series.Map<Series, SeriesView>();

            view.Links.Add(_linkRenderer.Render("GetSeriesById", RelTypes.Self, new { id = series.Id }));
            view.Links.Add(_linkRenderer.Render("GetBooksBySeries", RelTypes.Books, new { id = series.Id }));

            if (_userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render("UpdateSeries", RelTypes.Update, new { id = series.Id }));
                view.Links.Add(_linkRenderer.Render("DeleteSeries", RelTypes.Delete, new { id = series.Id }));
            }

            return view;
        }
    }
}

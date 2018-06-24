using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderGenres
    {
        ListView<GenreView> RenderResult(IEnumerable<Genre> genres);
    }

    public class GenresRenderer : IRenderGenres
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public GenresRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }
        public ListView<GenreView> RenderResult(IEnumerable<Genre> genres)
        {
            var items = genres.Select(g => g.Map<Genre, GenreView>());
            var view = new ListView<GenreView> { Items = items };
            view.Links.Add(_linkRenderer.Render("GetGenres", RelTypes.Self));

            if (_userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render("CreateGenre", RelTypes.Create));
            }

            return view;
        }
    }
}

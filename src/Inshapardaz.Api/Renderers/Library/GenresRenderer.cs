using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using ObjectMapper = Inshapardaz.Api.Helpers.ObjectMapper;

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
        private readonly IRenderGenre _genreRenderer;

        public GenresRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IRenderGenre genreRenderer)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _genreRenderer = genreRenderer;
        }

        public ListView<GenreView> RenderResult(IEnumerable<Genre> genres)
        {
            var items = genres.Select(g => _genreRenderer.RenderResult(g));
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

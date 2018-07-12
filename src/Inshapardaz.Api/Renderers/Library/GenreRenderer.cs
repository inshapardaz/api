using System;
using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;
using ObjectMapper = Inshapardaz.Api.Helpers.ObjectMapper;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderGenre
    {
        GenreView RenderResult(Genre genre);
    }

    public class GenreRenderer : IRenderGenre
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public GenreRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }
        public GenreView RenderResult(Genre genre)
        {
            var view = ObjectMapper.Map<Genre, GenreView>(genre);

            view.Links.Add(_linkRenderer.Render("GetGenreById", RelTypes.Self, new { id = genre.Id }));
            view.Links.Add(_linkRenderer.Render("GetBooksByGenre", RelTypes.Books, new { id = genre.Id }));

            if (_userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render("UpdateGenre", RelTypes.Update, new { id = genre.Id }));
                view.Links.Add(_linkRenderer.Render("DeleteGenre", RelTypes.Delete, new { id = genre.Id }));
            }

            return view;
        }
    }
}

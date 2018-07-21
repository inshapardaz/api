﻿using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderAuthor
    {
        AuthorView Render(Author source);
    }

    public class AuthorRenderer : IRenderAuthor
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public AuthorRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public AuthorView Render(Author source)
        {
            var result = source.Map<Author, AuthorView>();

            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetAuthorById", RelTypes.Self, new { id = source.Id }),
                _linkRenderer.Render("GetBooksByAuthor", RelTypes.Books, new { id = source.Id })
            };

            if (source.ImageId > 0)
            {
                links.Add(_linkRenderer.Render("GetFileById", RelTypes.Image, new { id = source.ImageId }));
            }

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateAuthor", RelTypes.Update, new { id = source.Id }));
                links.Add(_linkRenderer.Render("DeleteAuthor", RelTypes.Delete, new { id = source.Id }));
                links.Add(_linkRenderer.Render("UpdateAuthorImage", RelTypes.ImageUpload, new { id = source.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}
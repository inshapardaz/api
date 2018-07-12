using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using ObjectMapper = Inshapardaz.Api.Helpers.ObjectMapper;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderBook
    {
        BookView Render(Book source);
    }

    public class BookRenderer : IRenderBook
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IRenderEnum _enumRenderer;

        public BookRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IRenderEnum enumRenderer)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _enumRenderer = enumRenderer;
        }

        public BookView Render(Book source)
        {
            var result = ObjectMapper.Map<Book, BookView>(source);
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetBookById", RelTypes.Self, new { id = source.Id }),
                _linkRenderer.Render("GetAuthorById", RelTypes.Author, new { id = source.AuthorId })
            };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateBook", RelTypes.Update, new { id = source.Id }));
                links.Add(_linkRenderer.Render("DeleteBook", RelTypes.Delete, new { id = source.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}

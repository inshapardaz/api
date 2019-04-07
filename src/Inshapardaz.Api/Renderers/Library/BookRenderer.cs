using System.Collections.Generic;
using System.IO;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

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
        private readonly IRenderCategory _categoryRenderer;

        public BookRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IRenderCategory categoryRenderer)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _categoryRenderer = categoryRenderer;
        }

        public BookView Render(Book source)
        {
            var result = source.Map<Book, BookView>();
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetBookById", RelTypes.Self, new { id = source.Id }),
                _linkRenderer.Render("GetAuthorById", RelTypes.Author, new { id = source.AuthorId }),
                _linkRenderer.Render("GetChaptersForBook", RelTypes.Chapters, new { bookId = source.Id })
            };

            if (source.SeriesId.HasValue)
            {
                _linkRenderer.Render("GetSeriesById", RelTypes.Series, new {id = source.Id});
            }

            if (source.ImageId > 0)
            {
                links.Add(_linkRenderer.Render("GetFileById", RelTypes.Image, new {id = source.ImageId, ext = string.Empty, height = 257, width = 182 }));
            }

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("UpdateBook", RelTypes.Update, new { id = source.Id }));
                links.Add(_linkRenderer.Render("DeleteBook", RelTypes.Delete, new { id = source.Id }));
                links.Add(_linkRenderer.Render("UpdateBookImage", RelTypes.ImageUpload, new { id = source.Id }));
                links.Add(_linkRenderer.Render("CreateChapter", RelTypes.CreateChapter, new { bookId = source.Id }));
            }

            result.Links = links;

            if (source.Categories != null)
            {
                var categories = new List<CategoryView>();
                foreach (var category in source.Categories)
                {
                    categories.Add(_categoryRenderer.RenderResult(category));
                }

                result.Categories = categories;
            }

            return result;
        }
    }
}

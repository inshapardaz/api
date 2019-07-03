using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Books.Chapters;
using Inshapardaz.Functions.Library.Books.Files;
using Inshapardaz.Functions.Library.Files;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Adapters.Library
{
    public interface IRenderBook
    {
        BookView Render(ClaimsPrincipal principal, Book source);
    }

    public class BookRenderer : IRenderBook
    {
        private readonly IRenderCategory _categoryRenderer;

        public BookRenderer(IRenderCategory categoryRenderer)
        {
            _categoryRenderer = categoryRenderer;
        }

        public BookView Render(ClaimsPrincipal principal, Book source)
        {
            var result = source.Map();
            var links = new List<LinkView>
            {
                GetBookById.Link(source.Id, RelTypes.Self),
                GetAuthorById.Link(source.AuthorId, RelTypes.Author),
                GetChaptersByBook.Link(source.Id, RelTypes.Chapters),
                GetBookFiles.Link(source.Id, RelTypes.Files)
            };

            if (source.SeriesId.HasValue)
            {
                GetSeriesById.Link(source.SeriesId.Value, RelTypes.Series);
            }

            if (source.ImageId > 0)
            {
                links.Add(GetFileById.Link(source.ImageId, RelTypes.Image));
            }

            if (principal.IsWriter())
            {
                links.Add(UpdateBook.Link(source.Id, RelTypes.Update));
                links.Add(DeleteBook.Link(source.Id, RelTypes.Delete));
                links.Add(UpdateBookImage.Link(source.Id, RelTypes.ImageUpload));
                links.Add(AddChapter.Link(source.Id, RelTypes.CreateChapter));
                links.Add(AddBookFile.Link(source.Id, RelTypes.AddFile));
            }

            result.Links = links;

            if (source.Categories != null)
            {
                var categories = new List<CategoryView>();
                foreach (var category in source.Categories)
                {
                    categories.Add(_categoryRenderer.Render(principal, category));
                }

                result.Categories = categories;
            }

            return result;
        }
    }
}

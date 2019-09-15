using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Books.Chapters;
using Inshapardaz.Functions.Library.Books.Files;
using Inshapardaz.Functions.Library.Files;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Converters
{
    public static class BooksRenderer
    {
        public static PageView<BookView> Render(this PageRendererArgs<Book> source, ClaimsPrincipal principal)
        {
            var page = new PageView<BookView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => x.Render(principal))
            };

            var links = new List<LinkView>
            {
                source.LinkFunc(page.CurrentPageIndex, page.PageSize, RelTypes.Self)
            };

            if (principal.IsWriter())
            {
                links.Add(AddBook.Link(RelTypes.Create));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(source.LinkFunc(page.CurrentPageIndex + 1, page.PageSize, RelTypes.Next));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                links.Add(source.LinkFunc(page.CurrentPageIndex - 1, page.PageSize, RelTypes.Previous));
            }

            page.Links = links;
            return page;
        }

        public static PageView<BookView> Render(this PageRendererArgs<Book> source, int id, ClaimsPrincipal principal)
        {
            var page = new PageView<BookView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => x.Render(principal))
            };

            var links = new List<LinkView>
            {
                source.LinkFuncWithParameter(id, page.CurrentPageIndex, page.PageSize, RelTypes.Self)
            };

            if (principal.IsWriter())
            {
                links.Add(AddBook.Link(RelTypes.Create));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(source.LinkFuncWithParameter(id, page.CurrentPageIndex + 1, page.PageSize, RelTypes.Next));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                links.Add(source.LinkFuncWithParameter(id, page.CurrentPageIndex - 1, page.PageSize, RelTypes.Previous));
            }

            page.Links = links;
            return page;
        }

        public static ListView<BookView> Render(this IEnumerable<Book> source, ClaimsPrincipal principal, Func<string, LinkView> selfLinkMethod)
        {
            var result = new ListView<BookView>()
            {
                Items = source.Select(x => x.Render(principal))

            };

            result.Links.Add(selfLinkMethod(RelTypes.Self));

            return result;
        }

        public static BookView Render(this Book source, ClaimsPrincipal principal)
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
                links.Add(GetSeriesById.Link(source.SeriesId.Value, RelTypes.Series));
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

            if (principal.IsAuthenticated())
            {
                if (source.IsFavorite)
                {
                    links.Add(DeleteBookFromFavorite.Link(source.Id, RelTypes.RemoveFavorite));
                }
                else
                {
                    links.Add(AddBookToFavorite.Link(source.Id, RelTypes.AddFavorite));
                }
            }

            result.Links = links;

            if (source.Categories != null)
            {
                var categories = new List<CategoryView>();
                foreach (var category in source.Categories)
                {
                    categories.Add(category.Render(principal));
                }

                result.Categories = categories;
            }

            return result;
        }
    }
}

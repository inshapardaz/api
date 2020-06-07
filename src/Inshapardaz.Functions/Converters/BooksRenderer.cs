using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Books.Chapters;
using Inshapardaz.Functions.Library.Books.Content;
using Inshapardaz.Functions.Library.Files;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Inshapardaz.Functions.Converters
{
    public static class BooksRenderer
    {
        public static PageView<BookView> Render(this PageRendererArgs<BookModel> source, int id, ClaimsPrincipal principal)
        {
            var page = new PageView<BookView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => x.Render(principal))
            };

            var links = new List<LinkView>
            {
                source.LinkFuncWithParameter(id, page.CurrentPageIndex, page.PageSize, source.RouteArguments.Query, RelTypes.Self)
            };

            if (principal.IsWriter())
            {
                links.Add(AddBook.Link(id, RelTypes.Create));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(source.LinkFuncWithParameter(id, page.CurrentPageIndex + 1, page.PageSize, source.RouteArguments.Query, RelTypes.Next));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                links.Add(source.LinkFuncWithParameter(id, page.CurrentPageIndex - 1, page.PageSize, source.RouteArguments.Query, RelTypes.Previous));
            }

            page.Links = links;
            return page;
        }

        public static ListView<BookView> Render(this IEnumerable<BookModel> source, ClaimsPrincipal principal, Func<int, string, LinkView> selfLinkMethod)
        {
            var result = new ListView<BookView>()
            {
                Data = source.Select(x => x.Render(principal)),
                Links = new List<LinkView>()
            };

            result.Links.Add(selfLinkMethod(0, RelTypes.Self));

            return result;
        }

        public static BookView Render(this BookModel source, ClaimsPrincipal principal)
        {
            var result = source.Map();
            var links = new List<LinkView>
            {
                GetBookById.Link(source.LibraryId, source.Id, RelTypes.Self),
                GetAuthorById.Link(source.LibraryId, source.AuthorId, RelTypes.Author),
                GetChaptersByBook.Link(source.LibraryId, source.Id, RelTypes.Chapters),
            };

            if (source.SeriesId.HasValue)
            {
                links.Add(GetSeriesById.Link(source.LibraryId, source.SeriesId.Value, RelTypes.Series));
            }

            if (!string.IsNullOrWhiteSpace(source.ImageUrl))
            {
                links.Add(new LinkView { Href = source.ImageUrl, Method = "GET", Rel = RelTypes.Image, Accept = MimeTypes.Jpg });
            }
            else if (source.ImageId.HasValue)
            {
                links.Add(GetFileById.Link(source.ImageId.Value, RelTypes.Image));
            }

            if (principal.IsWriter())
            {
                links.Add(UpdateBook.Link(source.LibraryId, source.Id, RelTypes.Update));
                links.Add(DeleteBook.Link(source.LibraryId, source.Id, RelTypes.Delete));
                links.Add(UpdateBookImage.Link(source.LibraryId, source.Id, RelTypes.ImageUpload));
                links.Add(AddChapter.Link(source.LibraryId, source.Id, RelTypes.CreateChapter));
                links.Add(AddBookContent.Link(source.LibraryId, source.Id, RelTypes.AddFile));
            }

            if (principal.IsAuthenticated())
            {
                if (source.IsFavorite)
                {
                    links.Add(DeleteBookFromFavorite.Link(source.LibraryId, source.Id, RelTypes.RemoveFavorite));
                }
                else
                {
                    links.Add(AddBookToFavorite.Link(source.LibraryId, RelTypes.CreateFavorite));
                }
            }

            result.Links = links;

            if (source.Categories != null)
            {
                var categories = new List<CategoryView>();
                foreach (var category in source.Categories)
                {
                    categories.Add(category.Render(source.LibraryId, principal));
                }

                result.Categories = categories;
            }

            if (source.Contents.Any())
            {
                var contents = new List<BookContentView>();
                foreach (var content in source.Contents)
                {
                    contents.Add(content.Render(source.LibraryId, principal));
                }

                result.Contents = contents;
            }

            return result;
        }

        public static BookContentView Render(this BookContentModel source, int libraryId, ClaimsPrincipal principal)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                GetBookContent.Link(libraryId, source.BookId, source.Language, source.MimeType,  RelTypes.Self),
                GetBookById.Link(libraryId, source.BookId, RelTypes.Book)
            };

            if (!string.IsNullOrWhiteSpace(source.ContentUrl))
            {
                links.Add(new LinkView { Href = source.ContentUrl, Method = "GET", Rel = RelTypes.Download, Accept = MimeTypes.Jpg });
            }

            if (principal.IsWriter())
            {
                links.Add(UpdateBookContent.Link(libraryId, source.BookId, source.MimeType, source.Language, RelTypes.Update));
                links.Add(DeleteBookContent.Link(libraryId, source.BookId, source.MimeType, source.Language, RelTypes.Delete));
            }

            result.Links = links;
            return result;
        }
    }
}

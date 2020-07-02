using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Books.Chapters;
using Inshapardaz.Functions.Library.Books.Chapters.Contents;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Inshapardaz.Functions.Converters
{
    public static class ChapterConverter
    {
        public static ListView<ChapterView> Render(this IEnumerable<ChapterModel> source, int libraryId, int bookId, ClaimsPrincipal principal)
        {
            var items = source.Select(c => c.Render(libraryId, principal)).ToList();
            var view = new ListView<ChapterView> { Data = items };
            view.Links.Add(GetChaptersByBook.Link(libraryId, bookId, RelTypes.Self));

            if (principal.IsWriter())
            {
                view.Links.Add(AddChapter.Link(libraryId, bookId, RelTypes.Create));
            }

            return view;
        }

        public static ChapterView Render(this ChapterModel source, int libraryId, ClaimsPrincipal principal)
        {
            var result = source.Map();
            var links = new List<LinkView>
            {
                GetChapterById.Link(libraryId, source.BookId, source.Id, RelTypes.Self),
                GetBookById.Link(libraryId, source.BookId, RelTypes.Book)
            };

            if (principal.IsWriter())
            {
                links.Add(UpdateChapter.Link(libraryId, source.BookId, source.Id, RelTypes.Update));
                links.Add(DeleteChapter.Link(libraryId, source.BookId, source.Id, RelTypes.Delete));
                links.Add(AddChapterContents.Link(libraryId, source.BookId, source.Id, RelTypes.AddContent));
            }

            if (principal.IsAuthenticated())
            {
                var contents = new List<ChapterContentView>();
                foreach (var content in source.Contents)
                {
                    contents.Add(content.Render(libraryId, principal));
                }

                result.Contents = contents;
            }

            result.Links = links;
            return result;
        }

        public static ChapterContentView Render(this ChapterContentModel source, int libraryId, ClaimsPrincipal principal)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                GetChapterContents.Link(libraryId, source.BookId, source.ChapterId, source.Id, RelTypes.Self, source.MimeType, source.Language),
                GetBookById.Link(libraryId, source.BookId, RelTypes.Book),
                GetChapterById.Link(libraryId, source.BookId, source.ChapterId, RelTypes.Chapter)
            };

            if (!string.IsNullOrWhiteSpace(source.ContentUrl))
            {
                links.Add(new LinkView { Href = source.ContentUrl, Method = "GET", Rel = RelTypes.Download, Accept = MimeTypes.Jpg });
            }

            if (principal.IsWriter())
            {
                links.Add(UpdateChapterContents.Link(libraryId, source.BookId, source.ChapterId, source.Id, source.MimeType, RelTypes.Update, source.Language));
                links.Add(DeleteChapterContents.Link(libraryId, source.BookId, source.ChapterId, source.Id, source.MimeType, RelTypes.Delete, source.Language));
            }

            result.Links = links;
            return result;
        }
    }
}

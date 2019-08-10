using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Books.Chapters;
using Inshapardaz.Functions.Library.Books.Chapters.Contents;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Converters
{
    public static class ChapterConverter 
    {
        public static ListView<ChapterView> Render(this IEnumerable<Chapter> source, int bookId, ClaimsPrincipal principal)
        {
            var items = source.Select(c => c.Render(principal));
            var view = new ListView<ChapterView> { Items = items };
            view.Links.Add(GetChaptersByBook.Link(bookId, RelTypes.Self));

            if (principal.IsWriter())
            {
                view.Links.Add(AddChapter.Link(bookId, RelTypes.Create));
            }

            return view;
        }

        public static ChapterView Render(this Chapter source, ClaimsPrincipal principal)
        {
            var result = source.Map();
            var links = new List<LinkView>
            {
                GetChapterById.Link(source.BookId, source.Id, RelTypes.Self),
                GetBookById.Link(source.BookId, RelTypes.Book)
            };

            if (source.Contents != null)
            {
                foreach (var content in source.Contents)
                {
                    links.Add(GetChapterContents.Link(source.BookId, source.Id, content.MimeType, RelTypes.Contents));
                }
            }

            if (principal.IsWriter())
            {
                links.Add(UpdateChapter.Link(source.BookId, source.Id, RelTypes.Update));
                links.Add(DeleteChapter.Link(source.BookId, source.Id, RelTypes.Delete));
                if (source.Contents == null || !source.Contents.Any())
                {
                    links.Add(AddChapterContents.Link(source.BookId, source.Id, RelTypes.AddContents));
                }
                else
                {
                    foreach (var content in source.Contents)
                    {
                        links.Add(UpdateChapterContents.Link(source.BookId, source.Id, content.MimeType, RelTypes.UpdateContents));
                        links.Add(DeleteChapterContents.Link(source.BookId, source.Id, content.Id, content.MimeType, RelTypes.DeleteContents));
                    }
                }
            }

            result.Links = links;
            return result;
        }

        public static ChapterContentView Render (this ChapterContent source, ClaimsPrincipal principal)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                GetChapterContents.Link(source.BookId, source.ChapterId, source.MimeType, RelTypes.Self),
                GetBookById.Link(source.BookId, RelTypes.Book),
                GetChapterById.Link(source.BookId, source.ChapterId, RelTypes.Chapter)
            };

            if (principal.IsWriter())
            {
                links.Add(UpdateChapterContents.Link(source.BookId, source.ChapterId, source.MimeType, RelTypes.Update));
                links.Add(DeleteChapterContents.Link(source.BookId, source.ChapterId, source.Id, source.MimeType, RelTypes.Delete));
            }

            result.Links = links;
            return result;

        }
    }
}

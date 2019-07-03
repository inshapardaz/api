using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Adapters.Library
{
    public interface IRenderBooks
    {
        PageView<BookView> Render(ClaimsPrincipal principal, PageRendererArgs<Book> source);

        PageView<BookView> Render(int id, ClaimsPrincipal principal, PageRendererArgs<Book> source);

        IEnumerable<BookView> Render(ClaimsPrincipal principal, IEnumerable<Book> source);
    }

    public class BooksRenderer : IRenderBooks
    {
        private readonly IRenderBook _bookRenderer;

        public BooksRenderer(IRenderBook bookRenderer)
        {
            _bookRenderer = bookRenderer;
        }

        public PageView<BookView> Render(ClaimsPrincipal principal, PageRendererArgs<Book> source)
        {
            var page = new PageView<BookView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => _bookRenderer.Render(principal, x))
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

            if (page.PageCount > 1 && page.CurrentPageIndex > 1)
            {
                links.Add(source.LinkFunc(page.CurrentPageIndex - 1, page.PageSize, RelTypes.Previous));
            }

            page.Links = links;
            return page;
        }

        public PageView<BookView> Render(int id, ClaimsPrincipal principal, PageRendererArgs<Book> source)
        {
            var page = new PageView<BookView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => _bookRenderer.Render(principal, x))
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

            if (page.PageCount > 1 && page.CurrentPageIndex > 1)
            {
                links.Add(source.LinkFuncWithParameter(id, page.CurrentPageIndex - 1, page.PageSize, RelTypes.Previous));
            }

            page.Links = links;
            return page;
        }

        public IEnumerable<BookView> Render(ClaimsPrincipal principal, IEnumerable<Book> source)
        {
            return source.Select(x => _bookRenderer.Render(principal, x));
        }
    }
}
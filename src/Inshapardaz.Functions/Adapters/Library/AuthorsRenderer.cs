using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Adapters.Library
{
    public interface IRenderAuthors
    {
        PageView<AuthorView> Render(ClaimsPrincipal principal, PageRendererArgs<Author> source);
    }

    public class AuthorsRenderer : IRenderAuthors
    {
        private readonly IRenderAuthor _authorRenderer;

        public AuthorsRenderer(IRenderAuthor authorRenderer)
        {
            _authorRenderer = authorRenderer;
        }

        public PageView<AuthorView> Render(ClaimsPrincipal principal, PageRendererArgs<Author> source)
        {
            var page = new PageView<AuthorView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => _authorRenderer.Render(principal, x))
            };

            
            var links = new List<LinkView>
            {
                source.NavigationLinkGenerator(page.CurrentPageIndex, page.PageSize, RelTypes.Self)
            };

            if (principal.IsWriter())
            {
                links.Add(AddAuthor.Link(RelTypes.Create));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(source.NavigationLinkGenerator(page.CurrentPageIndex + 1, page.PageSize, RelTypes.Next));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1)
            {
                links.Add(source.NavigationLinkGenerator(page.CurrentPageIndex - 1, page.PageSize, RelTypes.Previous));
            }

            page.Links = links;
            return page;
        }

        private PagedRouteArgs CreateRouteParameters(PageRendererArgs<Author> source, int pageNumber, int pageSize)
        {
            var args = source.RouteArguments ?? new PagedRouteArgs();
            args.PageNumber = pageNumber;
            args.PageSize = pageSize;
            return args;
        }
    }
}
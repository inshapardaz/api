using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.Renderers
{
    public class PageRendererArgs<T>
    {
        public string RouteName { get; set; }

        public Page<T> Page { get; set; }

        public PagedRouteArgs RouteArguments { get; set; }
    }

    public class PagedRouteArgs
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class DictionarSearchPageRouteArgs : PagedRouteArgs
    {
        public int Id { get; set; }

        public string Query { get; set; }
    }

    public class RouteWithTitlePageRouteArgs : PagedRouteArgs
    {
        public string Title { get; set; }
    }
}
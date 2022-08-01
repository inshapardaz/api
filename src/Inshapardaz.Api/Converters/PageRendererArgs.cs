using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Converters
{
    public class PageRendererArgs<T>
    {
        public Page<T> Page { get; set; }

        public PagedRouteArgs RouteArguments { get; set; }
    }

    public class PageRendererArgs<T, TFilters>
    {
        public Page<T> Page { get; set; }

        public PagedRouteArgs RouteArguments { get; set; }

        public TFilters Filters { get; set; }
    }

    public class PageRendererArgs<T, TFilters, TSortBy>
    {
        public Page<T> Page { get; set; }

        public PagedRouteArgs<TSortBy> RouteArguments { get; set; }

        public TFilters Filters { get; set; }
    }

    public class PagedRouteArgs
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string Query { get; set; }

        public BookSortByType SortBy { get; internal set; }

        public SortDirection SortDirection { get; internal set; }
        public int? AccountId { get; set; }
    }

    public class PagedRouteArgs<TSortBy>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string Query { get; set; }

        public TSortBy SortBy { get; internal set; }

        public SortDirection SortDirection { get; internal set; }
        public int? AccountId { get; set; }
    }
}

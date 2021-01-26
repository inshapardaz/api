using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Converters
{
    public class PageRendererArgs<T>
    {
        public Page<T> Page { get; set; }

        public PagedRouteArgs RouteArguments { get; set; }
    }

    public class PagedRouteArgs
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string Query { get; set; }

        public BookFilter BookFilter { get; set; }
        public BookSortByType SortBy { get; internal set; }
        public SortDirection SortDirection { get; internal set; }
        public int? AccountId { get; set; }
    }
}

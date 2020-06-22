using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Paramore.Darker;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Library.Books
{
    public class GetBooks : QueryBase
    {
        public GetBooks(IQueryProcessor queryProcessor)
        : base(queryProcessor)
        {
        }

        [FunctionName("GetBooks")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "library/{libraryId}/books")] HttpRequest req,
            int libraryId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            var query = GetQueryParameter<string>(req, "query", null);
            var pageNumber = GetQueryParameter(req, "pageNumber", 1);
            var pageSize = GetQueryParameter(req, "pageSize", 10);
            var filter = GetFilter(req);
            var sortBy = GetQueryParameter<string>(req, "sortby").ToEnum<BookSortByType>(BookSortByType.Title);
            var sortDirection = GetQueryParameter<string>(req, "sort").ToEnum<SortDirection>(SortDirection.Ascending);

            var request = new GetBooksQuery(libraryId, pageNumber, pageSize, principal.GetUserId())
            {
                Query = query,
                Filter = filter,
                SortBy = sortBy,
                SortDirection = sortDirection
            };
            var books = await QueryProcessor.ExecuteAsync(request, cancellationToken: token);

            var args = new PageRendererArgs<BookModel>
            {
                Page = books,
                RouteArguments = new PagedRouteArgs
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    BookFilter = filter,
                    Query = query,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                },
                LinkFuncWithParameterEx = Link,
            };

            return new OkObjectResult(args.Render(libraryId, principal));
        }

        public static LinkView Link(int libraryId, string relType = RelTypes.Self) => SelfLink($"library/{libraryId}/books", relType);

        public static LinkView Link(int libraryId, int pageNumber = 1, int pageSize = 10, string query = null, string relType = RelTypes.Self, BookFilter filter = null, BookSortByType sortBy = BookSortByType.Title, SortDirection sortDirection = SortDirection.Ascending)
        {
            var queryString = new Dictionary<string, string>
            {
                { "pageNumber", pageNumber.ToString()},
                { "pageSize", pageSize.ToString()}
            };

            if (query != null)
            {
                queryString.Add("query", query);
            }

            if (filter != null)
            {
                if (filter.AuthorId.HasValue)
                    queryString.Add("authorid", filter.AuthorId.Value.ToString());

                if (filter.SeriesId.HasValue)
                    queryString.Add("seriesid", filter.SeriesId.Value.ToString());

                if (filter.CategoryId.HasValue)
                    queryString.Add("categoryid", filter.CategoryId.Value.ToString());

                if (filter.Favorite.HasValue)
                    queryString.Add("favorite", bool.TrueString);

                if (filter.Read.HasValue)
                    queryString.Add("read", bool.TrueString);
            }

            if (sortBy != BookSortByType.Title)
                queryString.Add("sortby", sortBy.ToDescription());

            if (sortDirection != SortDirection.Ascending)
                queryString.Add("sort", sortDirection.ToDescription());

            return SelfLink($"library/{libraryId}/books", relType, queryString: queryString);
        }

        private BookFilter GetFilter(HttpRequest request)
        {
            // TODO : Somehow make it case-insensitive
            var authorId = GetQueryParameter<int>(request, "authorid");
            var seriesId = GetQueryParameter<int>(request, "seriesid");
            var categoryId = GetQueryParameter<int>(request, "categoryid");
            var favorite = GetQueryParameter<bool>(request, "favorite");
            var read = GetQueryParameter<bool>(request, "read");

            return new BookFilter()
            {
                AuthorId = authorId != 0 ? authorId : (int?)null,
                SeriesId = seriesId != 0 ? seriesId : (int?)null,
                CategoryId = categoryId != 0 ? categoryId : (int?)null,
                Favorite = favorite ? true : (bool?)null,
                Read = read ? true : (bool?)null
            };
        }
    }
}

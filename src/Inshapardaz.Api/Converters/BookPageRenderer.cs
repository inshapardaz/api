using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderBookPage
    {
        BookPageView Render(BookPageModel source, int libraryId);

        PageView<BookPageView> Render(PageRendererArgs<BookPageModel, BookPageFilter> source, int libraryId, int bookId);

        LinkView RenderImageLink(int imageId);
    }

    public class BookPageRenderer : IRenderBookPage
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public BookPageRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public PageView<BookPageView> Render(PageRendererArgs<BookPageModel, BookPageFilter> source, int libraryId, int bookId)
        {
            var page = new PageView<BookPageView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId))
            };

            Dictionary<string, string> query = CreateQueryString(source, page);
            query.Add("pageNumber", (page.CurrentPageIndex).ToString());

            var links = new List<LinkView>();

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookPageController.GetPagesByBook),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId, bookId = bookId },
                QueryString = query
            }));

            if (_userHelper.IsWriter || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.CreatePage),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId, bookId = bookId }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.UploadPages),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.MultiCreate,
                    Parameters = new { libraryId = libraryId, bookId = bookId }
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex + 1).ToString());

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.GetPagesByBook),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId, bookId = bookId },
                    QueryString = pageQuery
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex - 1).ToString());
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.GetPagesByBook),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId, bookId = bookId },
                    QueryString = pageQuery
                }));
            }

            page.Links = links;
            return page;
        }

        public BookPageView Render(BookPageModel source, int libraryId)
        {
            var result = source.Map();
            var links = new List<LinkView>
                    {
                        _linkRenderer.Render(new Link
                        {
                            ActionName = nameof(BookPageController.GetPageByIndex),
                            Method = HttpMethod.Get,
                            Rel = RelTypes.Self,
                            Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                        }),
                        _linkRenderer.Render(new Link
                        {
                            ActionName = nameof(BookController.GetBookById),
                            Method = HttpMethod.Get,
                            Rel = RelTypes.Book,
                            Parameters = new { libraryId = libraryId, bookId = source.BookId }
                        })
                    };
            if (source.ImageId.HasValue)
            {
                links.Add(RenderImageLink(source.ImageId.Value));
            }

            if (_userHelper.IsWriter || _userHelper.IsLibraryAdmin || _userHelper.IsAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.UpdatePage),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.DeletePage),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                }));

                if (_userHelper.IsLibraryAdmin || _userHelper.IsAdmin)
                {
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(BookPageController.AssignPage),
                        Method = HttpMethod.Post,
                        Rel = RelTypes.Assign,
                        Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                    }));
                }

                if (source.AccountId != _userHelper.Account.Id)
                {
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(BookPageController.AssignPageToUser),
                        Method = HttpMethod.Post,
                        Rel = RelTypes.AssignToMe,
                        Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                    }));
                }

                if (source.ImageId.HasValue)
                {
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(BookPageController.UpdatePageImage),
                        Method = HttpMethod.Put,
                        Rel = RelTypes.ImageUpload,
                        Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                    }));
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(BookPageController.DeletePageImage),
                        Method = HttpMethod.Delete,
                        Rel = RelTypes.ImageDelete,
                        Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                    }));
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(BookPageController.OcrPage),
                        Method = HttpMethod.Post,
                        Rel = RelTypes.Ocr,
                        Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                    }));
                }
                else
                {
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(BookPageController.UpdatePageImage),
                        Method = HttpMethod.Put,
                        Rel = RelTypes.ImageUpload,
                        Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                    }));
                }
            }

            result.Links = links;
            return result;
        }

        public LinkView RenderImageLink(int imageId) =>
            _linkRenderer.Render(new Link
            {
                ActionName = nameof(FileController.GetFile),
                Method = HttpMethod.Get,
                Rel = RelTypes.Image,
                Parameters = new { fileId = imageId }
            });

        private static Dictionary<string, string> CreateQueryString(PageRendererArgs<BookPageModel, BookPageFilter> source, PageView<BookPageView> page)
        {
            Dictionary<string, string> queryString = new Dictionary<string, string> {
                    { "pageSize", page.PageSize.ToString() }
                };

            if (!string.IsNullOrWhiteSpace(source.RouteArguments.Query))
            {
                queryString.Add("query", source.RouteArguments.Query);
            }

            if (source.Filters != null)
            {
                if (source.Filters.Status.HasValue && source.Filters.Status != Domain.Models.PageStatuses.All)
                {
                    queryString.Add("status", source.Filters.Status.Value.ToString());
                }

                if (source.Filters.AssignmentFilter.HasValue && source.Filters.AssignmentFilter != Domain.Models.AssignmentFilter.All)
                {
                    queryString.Add("assignmentFilter", source.Filters.AssignmentFilter.Value.ToString());

                    if (source.Filters.AssignmentFilter == Domain.Models.AssignmentFilter.AssignedTo &&
                        source.Filters.AccountId.HasValue)
                    {
                        queryString.Add("assignmentTo", source.Filters.AccountId.ToString());
                    }
                }
            }

            if (source.RouteArguments.SortBy != BookSortByType.Title)
                queryString.Add("sortby", source.RouteArguments.SortBy.ToDescription());

            if (source.RouteArguments.SortDirection != SortDirection.Ascending)
                queryString.Add("sort", source.RouteArguments.SortDirection.ToDescription());
            return queryString;
        }
    }
}

using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderBookPage
    {
        BookPageView Render(BookPageModel source, int libraryId);

        PageView<BookPageView> Render(PageRendererArgs<BookPageModel, PageFilter> source, int libraryId, int bookId);
        PageView<BookPageView> RenderUserPages(PageRendererArgs<BookPageModel, PageFilter> source, int libraryId);

        LinkView RenderImageLink(FileModel file);
    }

    public class BookPageRenderer : IRenderBookPage
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IFileStorage _fileStorage;

        public BookPageRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IFileStorage fileStorage)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _fileStorage = fileStorage;
        }

        public PageView<BookPageView> Render(PageRendererArgs<BookPageModel, PageFilter> source, int libraryId, int bookId)
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

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
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

        public PageView<BookPageView> RenderUserPages(PageRendererArgs<BookPageModel, PageFilter> source, int libraryId)
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
                ActionName = nameof(UserController.GetBookPagesByUser),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId },
                QueryString = query
            }));

            if (page.CurrentPageIndex < page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex + 1).ToString());

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(UserController.GetBookPagesByUser),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId },
                    QueryString = pageQuery
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex - 1).ToString());
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(UserController.GetBookPagesByUser),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId },
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

            if (!string.IsNullOrWhiteSpace(source.ImageUrl) && _fileStorage.SupportsPublicLink)
            {
                links.Add(new LinkView
                {
                    Href = _fileStorage.GetPublicUrl(source.ImageUrl),
                    Method = "GET",
                    Rel = RelTypes.Image,
                    Accept = MimeTypes.Jpg
                });
            }
            else if (source.ImageId.HasValue)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(FileController.GetFile),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Image,
                    Parameters = new { fileId = source.ImageId.Value }
                }));
            }

            if (source.PreviousPage != null)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.GetPageByIndex),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId, bookId = source.PreviousPage.BookId, sequenceNumber = source.PreviousPage.SequenceNumber }
                }));
            }

            if (source.NextPage != null)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.GetPageByIndex),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId, bookId = source.NextPage.BookId, sequenceNumber = source.NextPage.SequenceNumber }
                }));
            }

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsLibraryAdmin(libraryId) || _userHelper.IsAdmin)
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

                if (_userHelper.IsLibraryAdmin(libraryId) || _userHelper.IsAdmin)
                {
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(BookPageController.AssignPage),
                        Method = HttpMethod.Post,
                        Rel = RelTypes.Assign,
                        Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                    }));
                }

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.UpdatePageSequence),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.PageSequence,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, sequenceNumber = source.SequenceNumber }
                }));

                if (
                    ((source.Status == Domain.Models.EditingStatus.Available ||
                        source.Status == Domain.Models.EditingStatus.Typing) && 
                        source.WriterAccountId != _userHelper.Account.Id) || 
                    ((source.Status == Domain.Models.EditingStatus.Typed||
                        source.Status == Domain.Models.EditingStatus.InReview) &&
                        source.ReviewerAccountId != _userHelper.Account.Id)
                    )
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

        public LinkView RenderImageLink(FileModel file)
        {
            if (!string.IsNullOrWhiteSpace(file.FilePath) && _fileStorage.SupportsPublicLink)
            {
                return new LinkView
                {
                    Href = _fileStorage.GetPublicUrl(file.FilePath),
                    Method = "GET",
                    Rel = RelTypes.Image,
                    Accept = MimeTypes.Jpg
                };
            }

            return _linkRenderer.Render(new Link
            {
                ActionName = nameof(FileController.GetFile),
                Method = HttpMethod.Get,
                Rel = RelTypes.Image,
                Parameters = new { fileId = file.Id }
            });
        }

        private static Dictionary<string, string> CreateQueryString(PageRendererArgs<BookPageModel, PageFilter> source, PageView<BookPageView> page)
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
                if (source.Filters.Status.HasValue && source.Filters.Status != Domain.Models.EditingStatus.All)
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
                else if (source.Filters.ReviewerAssignmentFilter.HasValue && source.Filters.ReviewerAssignmentFilter != Domain.Models.AssignmentFilter.All)
                {
                    queryString.Add("reviewerAssignmentFilter", source.Filters.ReviewerAssignmentFilter.Value.ToString());

                    if (source.Filters.ReviewerAssignmentFilter == Domain.Models.AssignmentFilter.AssignedTo &&
                        source.Filters.AccountId.HasValue)
                    {
                        queryString.Add("assignmentTo", source.Filters.AccountId.ToString());
                    }
                }
            }

            if (source.RouteArguments.SortBy != BookSortByType.Title)
                queryString.Add("sortby", source.RouteArguments.SortBy.ToDescription());

            if (source.RouteArguments.SortDirection != SortDirection.Ascending)
                queryString.Add("sortDirection", source.RouteArguments.SortDirection.ToDescription());
            return queryString;
        }
    }
}

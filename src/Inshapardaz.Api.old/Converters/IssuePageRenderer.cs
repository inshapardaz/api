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
    public interface IRenderIssuePage
    {
        IssuePageView Render(IssuePageModel source, int libraryId);

        PageView<IssuePageView> Render(PageRendererArgs<IssuePageModel, PageFilter> source, int libraryId, int periodicalId, int volumeNumber, int issueNumber);
        PageView<IssuePageView> RenderUserPages(PageRendererArgs<IssuePageModel, PageFilter> source, int libraryId);

        LinkView RenderImageLink(int libraryId, FileModel file);
    }

    public class IssuePageRenderer : IRenderIssuePage
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IFileStorage _fileStorage;

        public IssuePageRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IFileStorage fileStorage)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _fileStorage = fileStorage;
        }

        public PageView<IssuePageView> Render(PageRendererArgs<IssuePageModel, PageFilter> source, int libraryId, int periodicalId, int volumeNumber, int issueNumber) 
        {
            var page = new PageView<IssuePageView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId))
            };

            Dictionary<string, string> query = CreateQueryString(source, page);
            query.Add("pageNumber", (page.CurrentPageIndex).ToString());

            var links = new List<LinkView>();

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(IssuePageController.GetPagesByIssue),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId, periodicalId, volumeNumber, issueNumber },
                QueryString = query
            }));

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.CreateIssuePage),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId, periodicalId, volumeNumber, issueNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.UploadIssuePages),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.MultiCreate,
                    Parameters = new { libraryId, periodicalId, volumeNumber, issueNumber }
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex + 1).ToString());

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.GetPagesByIssue),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId, periodicalId, volumeNumber, issueNumber },
                    QueryString = pageQuery
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex - 1).ToString());
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.GetPagesByIssue),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId, periodicalId, volumeNumber, issueNumber },
                    QueryString = pageQuery
                }));
            }

            page.Links = links;
            return page;
        }

        public PageView<IssuePageView> RenderUserPages(PageRendererArgs<IssuePageModel, PageFilter> source, int libraryId)
        {
            var page = new PageView<IssuePageView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId))
            };

            Dictionary<string, string> query = CreateQueryString(source, page);
            query.Add("pageNumber", (page.CurrentPageIndex).ToString());

            var links = new List<LinkView>();

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(UserController.GetIssuePagesByUser),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId },
                QueryString = query
            }));

            if (page.CurrentPageIndex < page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex + 1).ToString());

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(UserController.GetIssuePagesByUser),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId },
                    QueryString = pageQuery
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex - 1).ToString());
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(UserController.GetIssuePagesByUser),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId },
                    QueryString = pageQuery
                }));
            }

            page.Links = links;
            return page;
        }

        public IssuePageView Render(IssuePageModel source, int libraryId)
        {
            var result = source.Map();
            var links = new List<LinkView>
                    {
                        _linkRenderer.Render(new Link
                        {
                            ActionName = nameof(IssuePageController.GetIssuePageByIndex),
                            Method = HttpMethod.Get,
                            Rel = RelTypes.Self,
                            Parameters = new { 
                                libraryId = libraryId, 
                                periodicalId = source.PeriodicalId,
                                volumeNumber = source.VolumeNumber,
                                issueNumber = source.IssueNumber,
                                sequenceNumber = source.SequenceNumber
                            }
                        }),
                        _linkRenderer.Render(new Link
                        {
                            ActionName = nameof(IssueController.GetIssueById),
                            Method = HttpMethod.Get,
                            Rel = RelTypes.Issue,
                            Parameters = new { 
                                libraryId = libraryId,
                                periodicalId = source.PeriodicalId,
                                volumeNumber = source.VolumeNumber,
                                issueNumber = source.IssueNumber
                            }
                        }),
                        _linkRenderer.Render(new Link
                        {
                            ActionName = nameof(PeriodicalController.GetPeriodicalById),
                            Method = HttpMethod.Get,
                            Rel = RelTypes.Periodical,
                            Parameters = new {
                                libraryId = libraryId,
                                periodicalId = source.PeriodicalId
                            }
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
                    ActionName = nameof(FileController.GetLibraryFile),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Image,
                    Parameters = new { libraryId = libraryId, fileId = source.ImageId.Value }
                }));
            }

            if (source.PreviousPage != null)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.GetIssuePageByIndex),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new {
                        libraryId = libraryId,
                        periodicalId = source.PreviousPage.PeriodicalId,
                        volumeNumber = source.PreviousPage.VolumeNumber,
                        issueNumber = source.PreviousPage.IssueNumber,
                        sequenceNumber = source.PreviousPage.SequenceNumber 
                    }
                }));
            }

            if (source.NextPage != null)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.GetIssuePageByIndex),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { 
                        libraryId = libraryId,
                        periodicalId = source.NextPage.PeriodicalId,
                        volumeNumber = source.NextPage.VolumeNumber,
                        issueNumber = source.NextPage.IssueNumber,
                        sequenceNumber = source.NextPage.SequenceNumber 
                    }
                }));
            }

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsLibraryAdmin(libraryId) || _userHelper.IsAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.UpdateIssuePage),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new {
                        libraryId = libraryId,
                        periodicalId = source.PeriodicalId,
                        volumeNumber = source.VolumeNumber,
                        issueNumber = source.IssueNumber,
                        sequenceNumber = source.SequenceNumber
                    }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.DeleteIssuePage),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new {
                        libraryId = libraryId,
                        periodicalId = source.PeriodicalId,
                        volumeNumber = source.VolumeNumber,
                        issueNumber = source.IssueNumber,
                        sequenceNumber = source.SequenceNumber
                    }
                }));

                if (_userHelper.IsLibraryAdmin(libraryId) || _userHelper.IsAdmin)
                {
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(IssuePageController.AssignIssuePage),
                        Method = HttpMethod.Post,
                        Rel = RelTypes.Assign,
                        Parameters = new {
                            libraryId = libraryId,
                            periodicalId = source.PeriodicalId,
                            volumeNumber = source.VolumeNumber,
                            issueNumber = source.IssueNumber,
                            sequenceNumber = source.SequenceNumber
                        }
                    }));
                }

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.UpdateIssuePageSequence),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.PageSequence,
                    Parameters = new {
                        libraryId = libraryId,
                        periodicalId = source.PeriodicalId,
                        volumeNumber = source.VolumeNumber,
                        issueNumber = source.IssueNumber,
                        sequenceNumber = source.SequenceNumber
                    }
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
                        ActionName = nameof(IssuePageController.AssignIssuePageToUser),
                        Method = HttpMethod.Post,
                        Rel = RelTypes.AssignToMe,
                        Parameters = new {
                            libraryId = libraryId,
                            periodicalId = source.PeriodicalId,
                            volumeNumber = source.VolumeNumber,
                            issueNumber = source.IssueNumber,
                            sequenceNumber = source.SequenceNumber
                        }
                    }));
                }

                if (source.ImageId.HasValue)
                {
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(IssuePageController.UpdateIssuePageImage),
                        Method = HttpMethod.Put,
                        Rel = RelTypes.ImageUpload,
                        Parameters = new {
                            libraryId = libraryId,
                            periodicalId = source.PeriodicalId,
                            volumeNumber = source.VolumeNumber,
                            issueNumber = source.IssueNumber,
                            sequenceNumber = source.SequenceNumber
                        }
                    }));
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(IssuePageController.DeleteIssuePageImage),
                        Method = HttpMethod.Delete,
                        Rel = RelTypes.ImageDelete,
                        Parameters = new {
                            libraryId = libraryId,
                            periodicalId = source.PeriodicalId,
                            volumeNumber = source.VolumeNumber,
                            issueNumber = source.IssueNumber,
                            sequenceNumber = source.SequenceNumber
                        }
                    }));
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(IssuePageController.OcrIssuePage),
                        Method = HttpMethod.Post,
                        Rel = RelTypes.Ocr,
                        Parameters = new {
                            libraryId = libraryId,
                            periodicalId = source.PeriodicalId,
                            volumeNumber = source.VolumeNumber,
                            issueNumber = source.IssueNumber,
                            sequenceNumber = source.SequenceNumber
                        }
                    }));
                }
                else
                {
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(IssuePageController.UpdateIssuePageImage),
                        Method = HttpMethod.Put,
                        Rel = RelTypes.ImageUpload,
                        Parameters = new {
                            libraryId = libraryId,
                            periodicalId = source.PeriodicalId,
                            volumeNumber = source.VolumeNumber,
                            issueNumber = source.IssueNumber,
                            sequenceNumber = source.SequenceNumber
                        }
                    }));
                }
            }

            result.Links = links;
            return result;
        }

        public LinkView RenderImageLink(int libraryId, FileModel file)
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
                ActionName = nameof(FileController.GetLibraryFile),
                Method = HttpMethod.Get,
                Rel = RelTypes.Image,
                Parameters = new { libraryId = libraryId, fileId = file.Id }
            });
        }

        private static Dictionary<string, string> CreateQueryString(PageRendererArgs<IssuePageModel, PageFilter> source, PageView<IssuePageView> page)
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

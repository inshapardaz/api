using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderIssue
    {
        PageView<IssueView> Render(PageRendererArgs<IssueModel, IssueFilter, IssueSortByType> source, int libraryId, int periodicalId);

        IssueView Render(IssueModel source, int libraryId);

        IssueContentView Render(IssueContentModel source, int libraryId);
    }

    public class IssueRenderer : IRenderIssue
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IFileStorage _fileStorage;
        private readonly IRenderAuthor _authorRenderer;

        public IssueRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IFileStorage fileStorage, IRenderAuthor authorRenderer)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _fileStorage = fileStorage;
            _authorRenderer = authorRenderer;
        }

        public PageView<IssueView> Render(PageRendererArgs<IssueModel, IssueFilter, IssueSortByType> source, int libraryId, int periodicalId)
        {
            var page = new PageView<IssueView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId))
            };

            Dictionary<string, string> query = CreateQueryString(source, page);

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link {
                    ActionName = nameof(IssueController.GetIssues),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId, periodicalId },
                    QueryString = query
                })
            };

            if (_userHelper.IsWriter(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.CreateIssue),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId, periodicalId }
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex + 1).ToString());

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.GetIssues),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId, periodicalId },
                    QueryString = pageQuery
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex - 1).ToString());
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.GetIssues),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId },
                    QueryString = pageQuery
                }));
            }

            page.Links = links;
            return page;
        }

        public IssueView Render(IssueModel source, int libraryId)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.GetIssueById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(PeriodicalController.GetPeriodicalById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Periodical,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticlesByIssue),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Articles,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.GetPagesByIssue),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Pages,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
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

            if (_userHelper.IsWriter(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.UpdateIssue),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.DeleteIssue),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.UpdateIssueImage),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.ImageUpload,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber,  issueNumber = source.IssueNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.CreateArticle),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.CreateArticle,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssuePageController.CreateIssuePage),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.AddPages,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.CreateIssueContent),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.AddContent,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
                }));
            }

            if (source.Authors.Any())
            {
                var authors = new List<AuthorView>();
                foreach (var author in source.Authors)
                {
                    authors.Add(_authorRenderer.Render(author, libraryId));
                }

                result.Authors = authors;
            }

            if (source.Contents.Any())
            {
                var contents = new List<IssueContentView>();
                foreach (var content in source.Contents)
                {
                    contents.Add(Render(content, libraryId));
                }

                result.Contents = contents;
            }

            result.Links = links;
            return result;
        }

        public IssueContentView Render(IssueContentModel source, int libraryId)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link {
                    ActionName = nameof(FileController.GetFile),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Language = source.Language,
                    MimeType = source.MimeType,
                    Parameters = new { fileId = source.FileId }
                }),
                _linkRenderer.Render(new Link {
                    ActionName = nameof(PeriodicalController.GetPeriodicalById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Periodical,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId }
                }),
                // TODO : Fix the issue number
                //_linkRenderer.Render(new Link {
                //    ActionName = nameof(IssueController.GetIssueById),
                //    Method = HttpMethod.Get,
                //    Rel = RelTypes.Issue,
                //    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNu = source.IssueId }
                //})
            };

            if (!string.IsNullOrWhiteSpace(source.ContentUrl))
            {
                // TODO :  Check and add direct link
                //links.Add(new LinkView { Href = source.ContentUrl, Method = "GET", Rel = RelTypes.Download, Accept = MimeTypes.Jpg });
            }

            if (_userHelper.IsWriter(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.UpdateIssueContent),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Language = source.Language,
                    MimeType = source.MimeType,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.DeleteIssueContent),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Update,
                    Language = source.Language,
                    MimeType = source.MimeType,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
                }));
            }

            result.Links = links;
            return result;
        }

        private static Dictionary<string, string> CreateQueryString(PageRendererArgs<IssueModel, IssueFilter, IssueSortByType> source, PageView<IssueView> page)
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
                if (source.Filters.Year.HasValue)
                    queryString.Add("year", source.Filters.Year.Value.ToString());

                if (source.Filters.VolumeNumber.HasValue)
                    queryString.Add("volumeNumber", source.Filters.VolumeNumber.Value.ToString());
            }

            if (source.RouteArguments.SortBy != IssueSortByType.IssueDate)
                queryString.Add("sortby", source.RouteArguments.SortBy.ToDescription());

            if (source.RouteArguments.SortDirection != SortDirection.Ascending)
                queryString.Add("sortDirection", source.RouteArguments.SortDirection.ToDescription());
            return queryString;
        }
    }
}

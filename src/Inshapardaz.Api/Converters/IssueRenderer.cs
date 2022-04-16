using Inshapardaz.Api.Controllers;
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
        PageView<IssueView> Render(PageRendererArgs<IssueModel> source, int libraryId, int periodicalId);

        IssueView Render(IssueModel source, int libraryId);

        IssueContentView Render(IssueContentModel source, int libraryId);
    }

    public class IssueRenderer : IRenderIssue
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IFileStorage _fileStorage;

        public IssueRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IFileStorage fileStorage)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _fileStorage = fileStorage;
        }

        public PageView<IssueView> Render(PageRendererArgs<IssueModel> source, int libraryId, int periodicalId)
        {
            var page = new PageView<IssueView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId))
            };

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link {
                    ActionName = nameof(IssueController.GetIssues),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId, periodicalId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , page.CurrentPageIndex.ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
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
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.GetIssues),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId, periodicalId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , (page.CurrentPageIndex + 1).ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.GetIssues),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , (page.CurrentPageIndex - 1).ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
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
                    Parameters = new { libraryId = libraryId, PeriodicalId = source.PeriodicalId, IssueNumber = source.IssueNumber }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticlesByIssue),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Articles,
                    Parameters = new { libraryId = libraryId, PeriodicalId = source.PeriodicalId, IssueNumber = source.IssueNumber }
                })
            };

            if (!string.IsNullOrWhiteSpace(source.ImageUrl))

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
                    Parameters = new { libraryId = libraryId, PeriodicalId = source.PeriodicalId, IssueNumber = source.IssueNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.DeleteIssue),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, PeriodicalId = source.PeriodicalId, IssueNumber = source.IssueNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.UpdateIssueImage),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.ImageUpload,
                    Parameters = new { libraryId = libraryId, PeriodicalId = source.PeriodicalId, IssueNumber = source.IssueNumber }
                }));
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
                _linkRenderer.Render(new Link {
                    ActionName = nameof(IssueController.GetIssueById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Issue,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, IssueNumber = source.IssueId }
                })
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
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, issueId = source.IssueId }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.DeleteIssueContent),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Update,
                    Language = source.Language,
                    MimeType = source.MimeType,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, issueId = source.IssueId }
                }));
            }

            result.Links = links;
            return result;
        }
    }
}

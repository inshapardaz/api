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
    public interface IRenderPeriodical
    {
        PageView<PeriodicalView> Render(PageRendererArgs<PeriodicalModel, PeriodicalFilter, PeriodicalSortByType> source, int libraryId);

        PeriodicalView Render(PeriodicalModel source, int libraryId);
    }

    public class PeriodicalRenderer : IRenderPeriodical
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IRenderCategory _categoryRenderer;
        private readonly IFileStorage _fileStorage;

        public PeriodicalRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IRenderCategory categoryRenderer, IFileStorage fileStorage)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _fileStorage = fileStorage;
            _categoryRenderer = categoryRenderer;
        }

        public PageView<PeriodicalView> Render(PageRendererArgs<PeriodicalModel, PeriodicalFilter, PeriodicalSortByType> source, int libraryId)
        {
            var page = new PageView<PeriodicalView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId))
            };

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link {
                    ActionName = nameof(PeriodicalController.GetPeriodicals),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , page.CurrentPageIndex.ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
                })
            };

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(PeriodicalController.CreatePeriodical),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId }
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(PeriodicalController.GetPeriodicals),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId },
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
                    ActionName = nameof(PeriodicalController.GetPeriodicals),
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

        public PeriodicalView Render(PeriodicalModel source, int libraryId)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(PeriodicalController.GetPeriodicalById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId, periodicalId = source.Id }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.GetIssues),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Issues,
                    Parameters = new { libraryId = libraryId, periodicalId = source.Id },
                    QueryString = new Dictionary<string, string>
                    {
                        { "authorid", source.Id.ToString() }
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
            else if(source.ImageId.HasValue)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(FileController.GetFile),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Image,
                    Parameters = new { fileId = source.ImageId.Value }
                }));
            }

            if (source.Categories != null)
            {
                var categories = new List<CategoryView>();
                foreach (var category in source.Categories)
                {
                    categories.Add(_categoryRenderer.Render(category, source.LibraryId));
                }

                result.Categories = categories;
            }

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(PeriodicalController.UpdatePeriodical),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, periodicalId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(PeriodicalController.DeletePeriodical),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, periodicalId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.CreateIssue),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.CreateIssue,
                    Parameters = new { libraryId = libraryId, periodicalId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(PeriodicalController.UpdatePeriodicalImage),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.ImageUpload,
                    Parameters = new { libraryId = libraryId, periodicalId = source.Id }
                }));
            }

            result.Links = links;
            return result;
        }
    }
}

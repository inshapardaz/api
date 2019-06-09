using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderIssues
    {
        PageView<IssueView> Render(PageRendererArgs<Issue> source);
    }

    public class IssuesRenderer : IRenderIssues
    {
        private readonly IRenderIssue _issueRenderer;
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public IssuesRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IRenderIssue issueRenderer)
        {
            _issueRenderer = issueRenderer;
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public PageView<IssueView> Render(PageRendererArgs<Issue> source)
        {
            var page = new PageView<IssueView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => _issueRenderer.Render(x))
            };

            var links = new List<LinkView>
            {
                _linkRenderer.Render(source.RouteName, RelTypes.Self,
                                     CreateRouteParameters(source, page.CurrentPageIndex, page.PageSize))
            };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("CreateIssue", RelTypes.Create));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(_linkRenderer.Render(source.RouteName, RelTypes.Next,
                                               CreateRouteParameters(source, page.CurrentPageIndex + 1, page.PageSize)));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1)
            {
                links.Add(_linkRenderer.Render(source.RouteName, RelTypes.Previous,
                                               CreateRouteParameters(source, page.CurrentPageIndex - 1, page.PageSize)));
            }

            page.Links = links;
            return page;
        }

        private object CreateRouteParameters(PageRendererArgs<Issue> source, int pageNumber, int pageSize)
        {
            var args = source.RouteArguments ?? new PagedRouteArgs();
            args.PageNumber = pageNumber;
            args.PageSize = pageSize;
            return args;
        }
    }
}
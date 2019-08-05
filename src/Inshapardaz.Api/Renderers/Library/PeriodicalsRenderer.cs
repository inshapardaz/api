﻿using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderPeriodicals
    {
        PageView<PeriodicalView> Render(PageRendererArgs<Periodical> source);

        IEnumerable<PeriodicalView> Render(IEnumerable<Periodical> source);
    }

    public class PeriodicalsRenderer : IRenderPeriodicals
    {
        private readonly IRenderPeriodical _periodicalRenderer;
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public PeriodicalsRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IRenderPeriodical periodicalRenderer)
        {
            _periodicalRenderer = periodicalRenderer;
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public PageView<PeriodicalView> Render(PageRendererArgs<Periodical> source)
        {
            var page = new PageView<PeriodicalView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => _periodicalRenderer.Render(x))
            };

            var links = new List<LinkView>
            {
                _linkRenderer.Render(source.RouteName, RelTypes.Self,
                                     CreateRouteParameters(source, page.CurrentPageIndex, page.PageSize))
            };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("CreatePeriodical", RelTypes.Create));
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

        public IEnumerable<PeriodicalView> Render(IEnumerable<Periodical> source)
        {
            return source.Select(x => _periodicalRenderer.Render(x));
        }

        private object CreateRouteParameters(PageRendererArgs<Periodical> source, int pageNumber, int pageSize)
        {
            var args = source.RouteArguments ?? new PagedRouteArgs();
            args.PageNumber = pageNumber;
            args.PageSize = pageSize;
            return args;
        }
    }
}
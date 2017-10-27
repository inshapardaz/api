﻿using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderWordPage
    {
        PageView<WordView> Render(PageRendererArgs<Word> source, int dictionaryId);
    }

    public class WordIndexPageRenderer : IRenderWordPage
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IRenderWord _wordRenderer;

        public WordIndexPageRenderer(IRenderLink linkRenderer, IRenderWord wordRenderer)
        {
            _linkRenderer = linkRenderer;
            _wordRenderer = wordRenderer;
        }

        public PageView<WordView> Render(PageRendererArgs<Word> source, int dictionaryId)
        {
            var page = new PageView<WordView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data.Select(x => _wordRenderer.Render(x, dictionaryId))
            };

            var links = new List<LinkView>
            {
                _linkRenderer.Render(source.RouteName, "self",
                    CreateRouteParameters(source, page.CurrentPageIndex, page.PageSize))
            };

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(_linkRenderer.Render(source.RouteName, "next",
                    CreateRouteParameters(source, page.CurrentPageIndex + 1, page.PageSize)));
            }

            if (page.CurrentPageIndex > 1)
            {
                links.Add(_linkRenderer.Render(source.RouteName, "previous",
                    CreateRouteParameters(source, page.CurrentPageIndex - 1, page.PageSize)));
            }

            page.Links = links;
            return page;
        }

        private object CreateRouteParameters(PageRendererArgs<Word> source, int pageNumber, int pageSize)
        {
            var args = source.RouteArguments ?? new PagedRouteArgs();
            args.PageNumber = pageNumber;
            args.PageSize = pageSize;
            return args;
        }
    }
}
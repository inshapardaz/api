using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public class WordIndexPageRenderer : RendrerBase, IRenderResponseFromObject<PageRendererArgs<Word>, PageView<WordView>>
    {
        private readonly IRenderResponseFromObject<Word, WordView> _wordIndexRenderer;

        public WordIndexPageRenderer(IRenderLink linkRenderer, IRenderResponseFromObject<Word, WordView> wordIndexRenderer)
            : base(linkRenderer)
        {
            _wordIndexRenderer = wordIndexRenderer;
        }

        public PageView<WordView> Render(PageRendererArgs<Word> source)
        {
            var page = new PageView<WordView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data.Select(x => _wordIndexRenderer.Render(x))
            };

            var links = new List<LinkView>
            {
                LinkRenderer.Render(source.RouteName, "self",
                    CreateRouteParameters(source, page.CurrentPageIndex, page.PageSize))
            };

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(LinkRenderer.Render(source.RouteName, "next",
                    CreateRouteParameters(source, page.CurrentPageIndex + 1, page.PageSize)));
            }

            if (page.CurrentPageIndex > 1)
            {
                links.Add(LinkRenderer.Render(source.RouteName, "previous",
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
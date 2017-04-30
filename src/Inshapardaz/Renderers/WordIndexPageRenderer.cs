using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.Model;
using Inshapardaz.Domain.Model;

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

            var links = new List<LinkView>();
            var parameters1 = new { pageNumber = page.CurrentPageIndex, pageSize = page.PageSize };
            links.Add(LinkRenderer.Render(source.RouteName, "self", parameters1));

            if (page.CurrentPageIndex < page.PageCount)
            {
                var parameters = new { pageNumber = page.CurrentPageIndex + 1, pageSize = page.PageSize };
                links.Add(LinkRenderer.Render(source.RouteName, "next", parameters));
            }

            if (page.CurrentPageIndex > 1)
            {
                var parameters = new { pageNumber = page.CurrentPageIndex - 1, pageSize = page.PageSize };
                links.Add(LinkRenderer.Render(source.RouteName, "previous", parameters));
            }

            page.Links = links;
            return page;
        }
    }
}

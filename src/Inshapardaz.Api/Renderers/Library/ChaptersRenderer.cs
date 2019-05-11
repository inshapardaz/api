using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderChapters
    {
        ListView<ChapterView> Render(int bookId, IEnumerable<Chapter> source);
    }

    public class ChaptersRenderer : IRenderChapters
    {
        private readonly IRenderChapter _chapterRenderer;
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public ChaptersRenderer(IRenderChapter chapterRenderer, IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _chapterRenderer = chapterRenderer;
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public ListView<ChapterView> Render(int bookId, IEnumerable<Chapter> source)
        {
            var items = source.Select(c => _chapterRenderer.Render(c));
            var view = new ListView<ChapterView> { Items = items };
            view.Links.Add(_linkRenderer.Render("GetChaptersForBook", RelTypes.Self, new { bookId = bookId }));

            if (_userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render("CreateChapter", RelTypes.Create, new { bookId = bookId }));
            }

            return view;
        }
    }
}

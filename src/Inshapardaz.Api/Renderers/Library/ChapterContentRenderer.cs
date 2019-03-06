using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderChapterContent
    {
        ChapterContentView Render(ChapterContent source);
    }

    public class ChapterContentRenderer : IRenderChapterContent
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public ChapterContentRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public ChapterContentView Render(ChapterContent source)
        {
            var result = source.Map<ChapterContent, ChapterContentView>();
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetChapterContents", RelTypes.Self, new { bookId = source.BookId, chapterId = source.ChapterId }),
                _linkRenderer.Render("GetBookById", RelTypes.Book, new { id = source.BookId }),
                _linkRenderer.Render("GetChapterById", RelTypes.Chapter, new { bookId = source.BookId, chapterId = source.ChapterId  }),
            };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("UpdateChapterContents", RelTypes.Update, new { bookId = source.BookId, chapterId = source.ChapterId }));
                //links.Add(_linkRenderer.Render("DeleteChapter", RelTypes.Delete, new { bookId = source.BookId, chapterId = source.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}

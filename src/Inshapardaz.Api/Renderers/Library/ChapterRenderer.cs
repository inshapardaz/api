using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderChapter
    {
        ChapterView Render(Chapter source);
    }

    public class ChapterRenderer : IRenderChapter
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IRenderEnum _enumRenderer;

        public ChapterRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IRenderEnum enumRenderer)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _enumRenderer = enumRenderer;
        }

        public ChapterView Render(Chapter source)
        {
            var result = source.Map<Chapter, ChapterView>();
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetChapterById", RelTypes.Self, new { bookId = source.BookId, chapterId = source.Id }),
                _linkRenderer.Render("GetBookById", RelTypes.Book, new { id = source.BookId }),
                _linkRenderer.Render("GetChapterContents", RelTypes.Contents, new { id = source.BookId, chapterId = source.Id })
            };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateChapter", RelTypes.Update, new { bookId = source.BookId, chapterId = source.Id }));
                links.Add(_linkRenderer.Render("DeleteChapter", RelTypes.Delete, new { bookId = source.BookId, chapterId = source.Id }));
                links.Add(_linkRenderer.Render("UpdateChapterContents", RelTypes.UpdateContents, new { bookId = source.BookId, chapterId = source.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}

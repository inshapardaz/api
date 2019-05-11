using System.Collections.Generic;
using System.Linq;
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

        public ChapterRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public ChapterView Render(Chapter source)
        {
            var result = source.Map<Chapter, ChapterView>();
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetChapterById", RelTypes.Self, new {bookId = source.BookId, chapterId = source.Id}),
                _linkRenderer.Render("GetBookById", RelTypes.Book, new {id = source.BookId}),
            };

            foreach (var content in source.Contents)
            {
                links.Add(_linkRenderer.Render("GetChapterContents", RelTypes.Contents, content.MimeType,  new {bookId = source.BookId, chapterId = source.Id}));
            }
            
            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("UpdateChapter", RelTypes.Update, new { bookId = source.BookId, chapterId = source.Id }));
                links.Add(_linkRenderer.Render("DeleteChapter", RelTypes.Delete, new { bookId = source.BookId, chapterId = source.Id }));
                if (!source.Contents.Any())
                {
                    links.Add(_linkRenderer.Render("AddChapterContents", RelTypes.AddContents, new { bookId = source.BookId, chapterId = source.Id }));
                }
                else
                {
                    foreach (var content in source.Contents)
                    {
                        links.Add(_linkRenderer.Render("UpdateChapterContents", RelTypes.UpdateContents, content.MimeType, new { bookId = source.BookId, chapterId = source.Id }));
                    }                    
                }
            }

            result.Links = links;
            return result;
        }
    }
}

using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderBookFiles
    {
        BookFilesView Render(int bookId, IEnumerable<File> source);
    }
    public class BookFilesRenderer : IRenderBookFiles
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IRenderBookFile _bookFileRenderer;

        public BookFilesRenderer(IRenderBookFile bookFileRenderer, IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _bookFileRenderer = bookFileRenderer;
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public BookFilesView Render(int bookId, IEnumerable<File> source)
        {
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetBookById", RelTypes.Self, new { id = bookId }),
            };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("AddBookFile", RelTypes.Add, new { id = bookId }));
            }

            var files = source.Select(x => _bookFileRenderer.Render(bookId, x));

            var retval = new BookFilesView()
            {
                BookId = bookId,
                Links = links,
                Files = files
            };

            return retval;

        }
    }
}

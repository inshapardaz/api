using System;
using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderBookFile
    {
        BookFileView Render(int bookId, File source);
    }

    public class BookFileRenderer : IRenderBookFile
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public BookFileRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public BookFileView Render(int bookId, File source)
        {
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetFileById", RelTypes.Self, source.MimeType, new {id = source.Id, ext = GetFileName(source)})
            };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("UpdateBookFile", RelTypes.Update, new { id = bookId, fileId = source.Id }));
                links.Add(_linkRenderer.Render("DeleteBookFile", RelTypes.Delete, new { id = bookId, fileId = source.Id }));
            }

            return new BookFileView()
            {
                MimeType = source.MimeType,
                Links = links
            };
        }

        string GetFileName(File file)
        {
            return System.IO.Path.GetExtension(file.FileName).Trim('.');
        }
    }
}

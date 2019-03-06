using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderFile
    {
        FileView Render(File file);
    }

    public class FileRenderer : IRenderFile
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public FileRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public FileView Render(File file)
        {
            var result = file.Map<File, FileView>();
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetFileById", RelTypes.Self, file.MimeType, file.Id)
            };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("DeleteFile", RelTypes.Delete, new { id = file.Id }));
            }

            result.Links = links;

            return result;
        }
    }
}

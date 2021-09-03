using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Collections.Generic;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderFile
    {
        FileView Render(FileModel source);
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

        public FileView Render(FileModel source)
        {
            var result = source.Map();
            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(FileController.GetFile),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { fileId = source.Id },
                })
            };

            if (_userHelper.IsAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(FileController.DeleteFile),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { fileId = source.Id },
                }));
            }

            result.Links = links;

            return result;
        }
    }
}

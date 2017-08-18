using System;
using System.Collections.Generic;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Renderers
{
    public class DownloadDictionaryModelRenderer : IRenderResponseFromObject<DownloadJobModel, DownloadDictionaryView>
    {
        private readonly IRenderLink _linkRenderer;

        public DownloadDictionaryModelRenderer(IRenderLink linkRenderer)
        {
            _linkRenderer = linkRenderer;
        }

        public DownloadDictionaryView Render(DownloadJobModel source)
        {
            return new DownloadDictionaryView
            {
                Links = new List<LinkView>
                {
                    _linkRenderer.Render("DownloadDictionary", "self", new
                    {
                        Id = source.Id, source.Type
                    })
                }
            };
            throw new NotImplementedException();
        }
    }
}
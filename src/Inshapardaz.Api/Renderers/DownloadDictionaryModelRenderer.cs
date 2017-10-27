using System;
using System.Collections.Generic;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderDownloadDictionary
    {
        DownloadDictionaryView Render(DownloadJobModel source);
    }

    public class DownloadDictionaryModelRenderer : IRenderDownloadDictionary
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
                        Id = source.Id,
                        source.Type
                    })
                }
            };
        }
    }
}
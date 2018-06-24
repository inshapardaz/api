using System.Collections.Generic;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;

namespace Inshapardaz.Api.Renderers.Dictionary
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
                    _linkRenderer.Render("DownloadDictionary", RelTypes.Self, new
                    {
                        source.Id
                    })
                }
            };
        }
    }
}
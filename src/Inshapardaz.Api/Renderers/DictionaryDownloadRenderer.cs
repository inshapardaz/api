using System;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderDictionaryDownload
    {
        DownloadDictionaryView Render(DownloadJobModel source);
    }

    public class DictionaryDownloadRenderer : IRenderDictionaryDownload
    {
        public DownloadDictionaryView Render(DownloadJobModel source)
        {
            return new DownloadDictionaryView()
            {
                
            };
        }
    }
}
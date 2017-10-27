using System;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderDictionaryDownload
    {
        DownloadDictionaryView Render(object source);
    }

    public class DictionaryDownloadRenderer : IRenderDictionaryDownload
    {
        public DownloadDictionaryView Render(object source)
        {
            return new DownloadDictionaryView()
            {
            };
        }
    }
}
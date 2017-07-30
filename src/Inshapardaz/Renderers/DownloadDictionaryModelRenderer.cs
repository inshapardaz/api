using System;
using System.Collections.Generic;
using Inshapardaz.Api.Model;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.Renderers
{
    public class DownloadDictionaryModelRenderer : IRenderResponseFromObject<DictionaryDownload, DownloadDictionaryView>
    {
        private readonly IRenderLink _linkRenderer;

        public DownloadDictionaryModelRenderer(IRenderLink linkRenderer)
        {
            _linkRenderer = linkRenderer;
        }

        public DownloadDictionaryView Render(DictionaryDownload source)
        {
            /*var linkView = _linkRenderer.Render("JobStatus", "status", new {id = source.JobId});
            return new DownloadDictionaryView
            {
                Links = new List<LinkView>
                {
                    _linkRenderer.Render("DownloadDictionary", "self", new {Id = source.DictionaryId, source.Format}),
                    linkView,
                }
            };*/
            throw new NotImplementedException();
        }
    }
}
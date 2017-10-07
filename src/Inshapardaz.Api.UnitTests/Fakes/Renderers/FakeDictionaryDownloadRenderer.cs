using System;
using System.Collections.Generic;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeDictionaryDownloadRenderer : IRenderResponseFromObject<DownloadJobModel, DownloadDictionaryView>
    {
        public DownloadDictionaryView Render(DownloadJobModel source)
        {
            return new DownloadDictionaryView
            {
                Links = new List<LinkView>
                {
                    new LinkView {Href = new Uri("http://test.url"), Rel = "self"}
                }
            };
        }
    }
}
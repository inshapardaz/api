using System.Collections.Generic;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeDictionariesRenderer : IRenderDictionaries
    {
        public DictionariesView Render(IEnumerable<Dictionary> source)
        {
            return new DictionariesView
            {
                Items = new DictionaryView[0],
                Links = new LinkView[0]
            };
        }
    }
}

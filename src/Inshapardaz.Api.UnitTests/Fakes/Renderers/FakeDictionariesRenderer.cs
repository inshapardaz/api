using System.Collections.Generic;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeDictionariesRenderer : IRenderDictionaries
    {
        public DictionariesView Render(IEnumerable<Dictionary> source, Dictionary<int, int> wordCounts)
        {
            return new DictionariesView
            {
                Items = new DictionaryView[0],
                Links = new LinkView[0]
            };
        }
    }
}

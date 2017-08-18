using System.Collections.Generic;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeDictionariesRenderer : IRenderResponseFromObject<IEnumerable<Domain.Model.Dictionary>, DictionariesView>
    {
        private readonly DictionariesView _response = new DictionariesView();

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

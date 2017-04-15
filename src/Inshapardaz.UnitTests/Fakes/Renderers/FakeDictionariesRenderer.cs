using Inshapardaz.Model;
using Inshapardaz.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.UnitTests.Fakes.Renderers
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

using Inshapardaz.Model;
using Inshapardaz.Renderers;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.UnitTests.Fakes.Renderers
{
    public class FakeDictionaryRenderer : IRenderResponseFromObject<Dictionary, DictionaryView>
    {
        private readonly DictionaryView _response = new DictionaryView();

        public DictionaryView Render(Dictionary source)
        {
            return _response;
        }
    }
}

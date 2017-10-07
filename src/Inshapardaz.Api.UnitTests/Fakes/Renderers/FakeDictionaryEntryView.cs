using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeDictionaryEntryRender : IRenderResponse<DictionaryEntryView>
    {
        DictionaryEntryView _dictionaryEntryView = new DictionaryEntryView();

        public FakeDictionaryEntryRender WithResponse(DictionaryEntryView dictionaryEntryView)
        {
            _dictionaryEntryView = dictionaryEntryView;
            return this;
        }

        public DictionaryEntryView Render()
        {
            return _dictionaryEntryView;
        }
    }
}

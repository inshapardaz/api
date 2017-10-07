using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeEntryRender : IRenderResponse<EntryView>
    {
        private EntryView _entryView = new EntryView();

        public bool WasRendered { get; internal set; }

        public FakeEntryRender WithResult(EntryView entryView)
        {
            _entryView = entryView;
            return this;
        }

        public EntryView Render()
        {
            WasRendered = true;
            return _entryView;
        }
    }
}

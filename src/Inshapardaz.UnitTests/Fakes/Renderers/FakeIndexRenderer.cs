using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeIndexRenderer : IRenderResponse<IndexView>
    {
        private IndexView _response = new IndexView();

        public FakeIndexRenderer WithResponse(IndexView indexView)
        {
            _response = indexView;
            return this;
        }

        public IndexView Render()
        {
            return _response;
        }
    }
}

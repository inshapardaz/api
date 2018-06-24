using System.Collections.Generic;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakePageRenderer : IRenderWordPage
    {
        private int _count;
        private int _size = 10;
        private int _currentIndex = 1;
        private List<WordView> _data;

        public PageView<WordView> Render(PageRendererArgs<Word> source, int dictionaryId)
        {
            return new PageView<WordView>(_count, _size, _currentIndex)
            {
                Data = _data ?? new List<WordView>()
            };
        }

        public FakePageRenderer WithCount(int count)
        {
            _count = count;
            return this;
        }

        public FakePageRenderer WithPageSize(int size)
        {
            _size = size;
            return this;
        }

        public FakePageRenderer WithCurrentPageIndex(int index)
        {
            _currentIndex = index;
            return this;
        }

        public FakePageRenderer WithData(List<WordView> data)
        {
            _data = data;
            return this;
        }
    }
}

using System;
using System.Collections.Generic;
using Inshapardaz.Model;
using Inshapardaz.Renderers;

namespace Inshapardaz.UnitTests.Fakes.Renderers
{
    public class FakePageRenderer<TModel, TView> : IRenderResponseFromObject<PageRendererArgs<TModel>, PageView<TView>>
    {
        private int _count;
        private int _size = 10;
        private int _currentIndex = 1;
        private List<TView> _data;

        public PageView<TView> Render(PageRendererArgs<TModel> source)
        {
            return new PageView<TView>(_count, _size, _currentIndex)
            {
                Data = _data ?? new List<TView>()
            };
        }

        public FakePageRenderer<TModel, TView> WithCount(int count)
        {
            _count = count;
            return this;
        }

        public FakePageRenderer<TModel, TView> WithPageSize(int size)
        {
            _size = size;
            return this;
        }

        public FakePageRenderer<TModel, TView> WithCurrentPageIndex(int index)
        {
            _currentIndex = index;
            return this;
        }

        public FakePageRenderer<TModel, TView> WithData(List<TView> data)
        {
            _data = data;
            return this;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Inshapardaz.Api.Views
{
    public class PageView<T> : ViewWithLinks
    {
        public PageView(long count, int pageSize, int currentPageIndex)
        {
            TotalCount = count;
            PageSize = pageSize;
            PageCount = Convert.ToInt32(Math.Ceiling((double)count / PageSize));
            CurrentPageIndex = currentPageIndex;

            if (PageCount == 0)
            {
                PageCount = 1;
            }
        }

        public int PageSize { get; set; }

        public int PageCount { get; set; }

        public int CurrentPageIndex { get; set; }

        public long TotalCount { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
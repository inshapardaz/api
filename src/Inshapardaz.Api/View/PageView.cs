// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Page.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   The page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Inshapardaz.Api.View
{
    /// <summary>
    /// The page.
    /// </summary>
    /// <typeparam name="T"> type of objects to page
    /// </typeparam>
    public class PageView<T>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PageView{T}"/> class.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="currentPageIndex">
        /// The current page index.
        /// </param>
        public PageView(int count, int pageSize, int currentPageIndex)
        {
            PageSize = pageSize;
            // ReSharper disable once PossibleLossOfFraction
            PageCount = Convert.ToInt32(Math.Ceiling((double)count / PageSize));
            CurrentPageIndex = currentPageIndex;

            if (PageCount == 0)
            {
                PageCount = 1;
            }
        }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page count.
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Gets or sets the current page index.
        /// </summary>
        public int CurrentPageIndex { get; set; }

        public IEnumerable<LinkView> Links { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public IEnumerable<T> Data { get; set; }
    }
}
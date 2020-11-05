using FluentAssertions;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Asserts
{
    internal class PagingAssert<T>
    {
        private HttpResponseMessage _response;
        private readonly LibraryDto _library;
        private PageView<T> _page;

        public IEnumerable<T> Data => _page.Data;

        public PagingAssert(HttpResponseMessage response, LibraryDto library)
        {
            _response = response;
            _library = library;
            _page = response.GetContent<PageView<T>>().Result;
        }

        internal void ShouldHaveSelfLink(string endingWith, string parameterName = null, string parameterValue = null)
        {
            _page.SelfLink()
                  .ShouldBeGet()
                  .ShouldHaveUrl(endingWith);

            if (parameterName != null)
                _page.SelfLink().ShouldHaveQueryParameter<string>(parameterName, parameterValue);
        }

        internal void ShouldHaveSelfLink(string format, int pageNumber, int pageSize = 10, string parameterName = null, string parameterValue = null)
        {
            var link = _page.SelfLink();
            link.ShouldBeGet()
                .ShouldHaveUrl(format)
                .ShouldHaveQueryParameter("pageSize", pageSize)
                .ShouldHaveQueryParameter("pageNumber", pageNumber);

            if (parameterName != null)
                link.ShouldHaveQueryParameter<string>(parameterName, parameterValue);
        }

        internal void ShouldNotHaveNextLink()
        {
            _page.Links.AssertLinkNotPresent("next");
        }

        internal void ShouldNotHavePreviousLink()
        {
            _page.Links.AssertLinkNotPresent("prev");
        }

        internal void ShouldNotHaveCreateLink()
        {
            _page.Links.AssertLinkNotPresent("create");
        }

        internal void ShouldHaveNextLink(string format, int pageNumber, int pageSize = 10, string parameterName = null, string parameterValue = null)
        {
            var link = _page.Links.AssertLink("next");
            link.ShouldBeGet()
                .ShouldHaveUrl(format)
                .ShouldHaveQueryParameter("pageSize", pageSize)
                .ShouldHaveQueryParameter("pageNumber", pageNumber);

            if (parameterName != null)
                link.ShouldHaveQueryParameter<string>(parameterName, parameterValue);
        }

        internal void ShouldHavePreviousLink(string format, int pageNumber, int pageSize = 10, string parameterName = null, string parameterValue = null)
        {
            var link = _page.Links.AssertLink("previous");
            link.ShouldBeGet()
                .ShouldHaveUrl(format)
                .ShouldHaveQueryParameter("pageSize", pageSize)
                .ShouldHaveQueryParameter("pageNumber", pageNumber);

            if (parameterName != null)
                link.ShouldHaveQueryParameter<string>(parameterName, parameterValue);
        }

        internal PagingAssert<T> ShouldHaveTotalCount(int totalCount)
        {
            _page.TotalCount.Should().Be(totalCount);
            return this;
        }

        internal PagingAssert<T> ShouldHaveItems(int itemCount)
        {
            _page.Data.Count().Should().Be(itemCount);
            return this;
        }

        internal PagingAssert<T> ShouldHavePageSize(int pageSize)
        {
            _page.PageSize.Should().Be(pageSize);
            return this;
        }

        internal PagingAssert<T> ShouldHavePage(int pageNumber)
        {
            _page.CurrentPageIndex.Should().Be(pageNumber);
            return this;
        }

        internal void ShouldHaveCreateLink(string endingWith)
        {
            _page.CreateLink()
                  .ShouldBePost()
                  .ShouldHaveUrl(endingWith);
        }

        internal void ShouldHaveNoData()
        {
            _page.Data.Should().BeEmpty();
        }
    }
}

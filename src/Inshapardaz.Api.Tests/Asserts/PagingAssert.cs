using FluentAssertions;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Asserts
{
    internal class PagingAssert<T>
    {
        private HttpResponseMessage _response;
        private PageView<T> _page;

        public IEnumerable<T> Data => _page.Data;

        public PagingAssert(HttpResponseMessage response)
        {
            _response = response;
            _page = response.GetContent<PageView<T>>().Result;
        }

        internal void ShouldHaveSelfLink(string endingWith, params KeyValuePair<string, string>[] parameters)
        {
            _page.SelfLink()
                  .ShouldBeGet()
                  .ShouldHaveUrl(endingWith);

            foreach (var param in parameters)
            {
                _page.SelfLink().ShouldHaveQueryParameter<string>(param.Key, param.Value);
            }
        }

        internal void ShouldHaveSelfLink(string format, int pageNumber, int pageSize = 10, params KeyValuePair<string, string>[] parameters)
        {
            var link = _page.SelfLink();
            link.ShouldBeGet()
                .ShouldHaveUrl(format)
                .ShouldHaveQueryParameter("pageSize", pageSize)
                .ShouldHaveQueryParameter("pageNumber", pageNumber);

            foreach (var param in parameters)
            {
                link.ShouldHaveQueryParameter<string>(param.Key, param.Value);
            }
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

        internal void ShouldHaveNextLink(string format, int pageNumber, int pageSize = 10, params KeyValuePair<string, string>[] parameters)
        {
            var link = _page.Links.AssertLink("next");
            link.ShouldBeGet()
                .ShouldHaveUrl(format)
                .ShouldHaveQueryParameter("pageSize", pageSize)
                .ShouldHaveQueryParameter("pageNumber", pageNumber);

            foreach (var param in parameters)
            {
                link.ShouldHaveQueryParameter<string>(param.Key, param.Value);
            }
        }

        internal void ShouldHavePreviousLink(string format, int pageNumber, int pageSize = 10, params KeyValuePair<string, string>[] parameters)
        {
            var link = _page.Links.AssertLink("previous");
            link.ShouldBeGet()
                .ShouldHaveUrl(format)
                .ShouldHaveQueryParameter("pageSize", pageSize)
                .ShouldHaveQueryParameter("pageNumber", pageNumber);

            foreach (var param in parameters)
            {
                link.ShouldHaveQueryParameter<string>(param.Key, param.Value);
            }
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

        internal PagingAssert<T> ShouldHaveSomeItems()
        {
            _page.Data.Should().NotBeEmpty();
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

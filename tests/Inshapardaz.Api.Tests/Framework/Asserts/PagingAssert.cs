using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class PagingAssert<T>
    {
        private HttpResponseMessage _response;
        private PageView<T> _page;

        public IEnumerable<T> Data => _page.Data;

        public PagingAssert()
        {
        }

        public PagingAssert<T> ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _page = response.GetContent<PageView<T>>().Result;
            return this;
        }

        public PagingAssert<T> ShouldHaveSelfLink(string endingWith, params KeyValuePair<string, string>[] parameters)
        {
            _page.SelfLink()
                  .ShouldBeGet()
                  .ShouldHaveUrl(endingWith);

            foreach (var param in parameters)
            {
                _page.SelfLink().ShouldHaveQueryParameter<string>(param.Key, param.Value);
            }
            return this;
        }

        public PagingAssert<T> ShouldHaveSelfLink(string format, int pageNumber, int pageSize = 10, params KeyValuePair<string, string>[] parameters)
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
            return this;
        }

        public PagingAssert<T> ShouldNotHaveNextLink()
        {
            _page.Links.AssertLinkNotPresent("next");
            return this;
        }

        public PagingAssert<T> ShouldNotHavePreviousLink()
        {
            _page.Links.AssertLinkNotPresent("prev");
            return this;
        }

        public PagingAssert<T> ShouldNotHaveCreateLink()
        {
            _page.Links.AssertLinkNotPresent("create");
            return this;
        }

        public PagingAssert<T> ShouldHaveNextLink(string format, int? pageNumber = null, int pageSize = 10, params KeyValuePair<string, string>[] parameters)
        {
            var link = _page.Links.AssertLink("next");
            link.ShouldBeGet()
                .ShouldHaveUrl(format)
                .ShouldHaveQueryParameter("pageSize", pageSize);

            if (pageNumber.HasValue)
            {
                link.ShouldHaveQueryParameter("pageNumber", pageNumber);
            }

            foreach (var param in parameters)
            {
                link.ShouldHaveQueryParameter<string>(param.Key, param.Value);
            }
            return this;
        }

        public PagingAssert<T> ShouldHavePreviousLink(string format, int? pageNumber = null, int pageSize = 10, params KeyValuePair<string, string>[] parameters)
        {
            var link = _page.Links.AssertLink("previous");
            link.ShouldBeGet()
                .ShouldHaveUrl(format)
                .ShouldHaveQueryParameter("pageSize", pageSize);

            if (pageNumber.HasValue)
            {
                link.ShouldHaveQueryParameter("pageNumber", pageNumber);
            }

            foreach (var param in parameters)
            {
                link.ShouldHaveQueryParameter<string>(param.Key, param.Value);
            }
            return this;
        }

        public PagingAssert<T> ShouldHaveTotalCount(int totalCount)
        {
            _page.TotalCount.Should().Be(totalCount);
            return this;
        }

        public PagingAssert<T> ShouldHavePageCount(int pageCount)
        {
            _page.PageCount.Should().Be(pageCount);
            return this;
        }


        public PagingAssert<T> ShouldHaveItems(int itemCount)
        {
            _page.Data.Count().Should().Be(itemCount);
            return this;
        }

        public PagingAssert<T> ShouldHaveSomeItems()
        {
            _page.Data.Should().NotBeEmpty();
            return this;
        }

        public PagingAssert<T> ShouldHavePageSize(int pageSize)
        {
            _page.PageSize.Should().Be(pageSize);
            return this;
        }

        public PagingAssert<T> ShouldHavePage(int pageNumber)
        {
            _page.CurrentPageIndex.Should().Be(pageNumber);
            return this;
        }

        public PagingAssert<T> ShouldHaveCreateLink(string endingWith)
        {
            _page.CreateLink()
                  .ShouldBePost()
                  .ShouldHaveUrl(endingWith);
            return this;
        }

        public PagingAssert<T> ShouldHaveNoData()
        {
            _page.Data.Should().BeEmpty();
            return this;
        }
    }
}

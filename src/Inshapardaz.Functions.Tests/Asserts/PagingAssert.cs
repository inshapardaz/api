using FluentAssertions;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Inshapardaz.Functions.Tests.Helpers;
using System;
using System.Collections.Generic;
using Inshapardaz.Functions.Tests.Dto;

namespace Inshapardaz.Functions.Tests.Asserts
{
    internal class PagingAssert<T>
    {
        private ObjectResult _response;
        private readonly LibraryDto _library;
        private PageView<T> _page;

        public IEnumerable<T> Data => _page.Data;

        public PagingAssert(ObjectResult response, LibraryDto library)
        {
            _response = response;
            _library = library;
            _page = response.Value as PageView<T>;
        }

        internal void ShouldHaveSelfLink(string endingWith, string parameterName = null, string parameterValue = null)
        {
            _page.SelfLink()
                  .ShouldBeGet()
                  .ShouldHaveUrl(endingWith);

            if (parameterName != null)
                _page.SelfLink().ShouldHaveQueryParameter<string>(parameterName, parameterValue);
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

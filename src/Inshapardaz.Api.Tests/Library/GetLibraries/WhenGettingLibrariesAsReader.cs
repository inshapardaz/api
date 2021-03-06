﻿using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.GetLibraries
{
    [TestFixture]
    public class WhenGettingLibrariesAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<LibraryView> _assert;

        public WhenGettingLibrariesAsReader()
            : base(Role.Reader, createLibrary: false)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            LibraryBuilder.AssignToUser(AccountId).Build(4);

            _response = await Client.GetAsync($"/libraries");
            _assert = new PagingAssert<LibraryView>(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink("/libraries");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNavigationLinks()
        {
            _assert.ShouldNotHaveNextLink();
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedLibraries()
        {
            var expectedItems = LibraryBuilder.Libraries.OrderBy(a => a.Name).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Name == item.Name);
                actual.ShouldMatch(item)
                     .ShouldNotHaveEditLinks()
                     .ShouldNotHaveCreateAuthorLink()
                     .ShouldNotHaveCreateBookLink()
                     .ShouldNotHaveCreateCategoryLink()
                     .ShouldNotHaveCreateSeriesLink();
            }
        }
    }
}

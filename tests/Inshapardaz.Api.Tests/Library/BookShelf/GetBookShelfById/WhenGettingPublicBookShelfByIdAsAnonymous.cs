﻿using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelfById
{
    [TestFixture]
    public class WhenGettingPublicBookShelfByIdAsAnonymous : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfDto _expected;
        private BookShelfAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            var series = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(account.Id).AsPublic().Build(1);
            _expected = series.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves/{_expected.Id}");
            _assert = Services.GetService<BookShelfAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveBooksLink()
        {
            _assert.ShouldHaveBooksLink();
        }

        [Test]
        public void ShouldNotHaveUpdateLink()
        {
            _assert.ShouldNotHaveUpdateLink();
        }

        [Test]
        public void ShouldNotHaveDeleteLink()
        {
            _assert.ShouldNotHaveDeleteLink();
        }

        [Test]
        public void ShouldNotHaveImageUploadLink()
        {
            _assert.ShouldNotHaveImageUploadLink();
        }

        [Test]
        public void ShouldReturnCorrectBookShelfData()
        {
            _assert.ShouldHaveCorrectBookShelfRetunred(_expected);
        }
    }
}

﻿using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.GetAuthors
{
    [TestFixture]
    public class WhenGettingAuthorsLastPage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<AuthorView> _assert;

        public WhenGettingAuthorsLastPage()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            AuthorBuilder.WithLibrary(LibraryId).WithBooks(3).Build(50);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/authors?pageNumber={5}&pageSize={10}");
            _assert = Services.GetService<PagingAssert<AuthorView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/authors");
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/authors", 4);
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldHaveCorrectAuthorData()
        {
            var expectedItems = AuthorBuilder.Authors.OrderBy(a => a.Name).Skip(4 * 10).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<AuthorAssert>().ForView(actual)
                    .ForLibrary(LibraryId)
                    .ShouldBeSameAs(item)
                    .WithBookCount(3)
                    .WithReadOnlyLinks()
                    .ShouldHavePublicImageLink();
            }
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetBooks
{
    [TestFixture]
    public class WhenGettingBooksPageThatDoesNotExist : LibraryTest<Functions.Library.Books.GetBooks>
    {
        private OkObjectResult _response;
        private PagingAssert<BookView> _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder()
                          .WithQueryParameter("pageNumber", 3)
                          .WithQueryParameter("pageSize", 10)
                          .Build();

            var builder = Container.GetService<BooksDataBuilder>();
            builder.WithLibrary(LibraryId).Build(20);

            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _assert = new PagingAssert<BookView>(_response, Library);
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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books");
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldNotHaveAnyBooks()
        {
            _assert.ShouldHaveNoData();
        }
    }
}

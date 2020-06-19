using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthors
{
    [TestFixture]
    public class WhenGettingAuthorsPageThatDoesNotExist : LibraryTest<Functions.Library.Authors.GetAuthors>
    {
        private AuthorsDataBuilder _builder;
        private OkObjectResult _response;
        private PagingAssert<AuthorView> _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder()
                .WithQueryParameter("pageNumber", 100)
                .WithQueryParameter("pageSize", 10)
                .Build();

            _builder = Container.GetService<AuthorsDataBuilder>();
            _builder.WithLibrary(LibraryId).WithBooks(3).Build(20);

            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _assert = new PagingAssert<AuthorView>(_response, Library);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/authors");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
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
        public void ShouldReturnNoData()
        {
            _assert.ShouldHaveNoData();
        }
    }
}

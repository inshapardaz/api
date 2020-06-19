using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthorById
{
    [TestFixture]
    public class WhenGettingAuthorById : LibraryTest<Functions.Library.Authors.GetAuthorById>
    {
        private AuthorsDataBuilder _builder;
        private OkObjectResult _response;
        private AuthorDto _expected;
        private AuthorAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _builder = Container.GetService<AuthorsDataBuilder>();
            var authors = _builder.WithLibrary(LibraryId).WithBooks(3).Build(4);
            _expected = authors.First();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _expected.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _assert = AuthorAssert.WithResponse(_response).InLibrary(LibraryId);
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
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveBooksLink()
        {
            _assert.ShouldHaveBooksLink();
        }

        [Test]
        public void ShouldHaveImageLink()
        {
            _assert.ShouldHavePublicImageLink();
        }

        [Test]
        public void ShouldReturnCorrectAuthorData()
        {
            _assert.ShouldHaveCorrectAuthorRetunred(_expected, DatabaseConnection);
        }
    }
}

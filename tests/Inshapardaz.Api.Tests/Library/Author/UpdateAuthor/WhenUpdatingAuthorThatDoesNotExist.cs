using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.UpdateAuthor
{
    [TestFixture]
    public class WhenUpdatingAuthorThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private AuthorView _author;
        private AuthorAssert _assert;

        public WhenUpdatingAuthorThatDoesNotExist() : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = new AuthorView { Name = RandomData.Name };

            _response = await Client.PutObject($"/libraries/{LibraryId}/authors/{_author.Id}", _author);
            _assert = Services.GetService<AuthorAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldHaveCreatedTheAuthor()
        {
            _assert.ShouldHaveSavedAuthor();
        }
    }
}

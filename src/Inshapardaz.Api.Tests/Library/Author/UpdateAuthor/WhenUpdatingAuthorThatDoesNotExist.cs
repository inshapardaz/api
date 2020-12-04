using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.UpdateAuthor
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
            _author = new AuthorView { Name = Random.Name };

            _response = await Client.PutObject($"/libraries/{LibraryId}/authors/{_author.Id}", _author);
            _assert = AuthorAssert.WithResponse(_response).InLibrary(LibraryId);
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
            _assert.ShouldHaveSavedAuthor(DatabaseConnection);
        }
    }
}
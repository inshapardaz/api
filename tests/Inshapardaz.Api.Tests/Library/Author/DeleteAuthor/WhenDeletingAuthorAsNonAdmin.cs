using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.DeleteAuthor
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingAuthorAsNonAdmin(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).Build(4);
            var expected = authors.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/authors/{expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}

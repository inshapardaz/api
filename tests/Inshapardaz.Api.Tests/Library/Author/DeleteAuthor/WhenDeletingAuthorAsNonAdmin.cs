using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.DeleteAuthor
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingAuthorAsNonAdmin : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingAuthorAsNonAdmin(Role Role)
            : base(Role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).Build(4);
            var expected = authors.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/authors/{expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}

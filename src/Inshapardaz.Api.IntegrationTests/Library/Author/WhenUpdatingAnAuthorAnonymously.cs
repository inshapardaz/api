using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View.Library;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Author
{
    [TestFixture]
    public class WhenUpdatingAnAuthorAnonymously : IntegrationTestBase
    {
        private Domain.Entities.Library.Author _author;
        private readonly Guid _userId = Guid.NewGuid();
        private AuthorView _updatedAuthor;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author31" });

            _updatedAuthor = new AuthorView {Id = _author.Id, Name = "Some New Name"};

            Response = await GetClient().PutJson($"api/authors/{_author.Id}", _updatedAuthor);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            AuthorDataHelper.Delete(_author.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
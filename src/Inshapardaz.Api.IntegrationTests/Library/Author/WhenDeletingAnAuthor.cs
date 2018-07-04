using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Author
{
    [TestFixture]
    public class WhenDeletingAnAuthor : IntegrationTestBase
    {
        private Domain.Entities.Library.Author _view;
        private Domain.Entities.Library.Author _author;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "Author1"});

            Response = await GetAdminClient(_userId).DeleteAsync($"api/authors/{_author.Id}");

            _view = AuthorDataHelper.Get(_author.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            AuthorDataHelper.Delete(_author.Id);
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Test]
        public void ShouldHaveDeletedTheAuthor()
        {
            _view.ShouldBeNull();
        }
    }
}
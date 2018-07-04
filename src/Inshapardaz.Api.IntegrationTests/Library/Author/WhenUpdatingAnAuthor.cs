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
    public class WhenUpdatingAnAuthor : IntegrationTestBase
    {
        private Domain.Entities.Library.Author _view;
        private Domain.Entities.Library.Author _author;
        private readonly Guid _userId = Guid.NewGuid();
        private AuthorView _updatedAuthor;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author223" });

            _updatedAuthor = new AuthorView {Id = _author.Id, Name = "Some New Name"};

            Response = await GetAdminClient(_userId).PutJson($"api/authors/{_author.Id}", _updatedAuthor);

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
        public void ShouldHaveUpdateTheTranslation()
        {
            _view.Name.ShouldBe(_updatedAuthor.Name);
        }
    }
}
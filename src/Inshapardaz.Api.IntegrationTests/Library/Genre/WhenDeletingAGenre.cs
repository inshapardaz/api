using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Genre
{
    [TestFixture]
    public class WhenDeletingAGenre : IntegrationTestBase
    {
        private Domain.Entities.Library.Genre _view;
        private Domain.Entities.Library.Genre _genre;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _genre = GenreDataHelper.Create("genre1");

            Response = await GetAdminClient(_userId).DeleteAsync($"api/genres/{_genre.Id}");

            _view = GenreDataHelper.Get(_genre.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            GenreDataHelper.Delete(_genre.Id);
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Test]
        public void ShouldHaveDeletedTheTranslation()
        {
            _view.ShouldBeNull();
        }
    }
}
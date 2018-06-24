using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View.Library;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Genre
{
    [TestFixture]
    public class WhenUpdatingAGenre : IntegrationTestBase
    {
        private Domain.Entities.Library.Genre _view;
        private Domain.Entities.Library.Genre _genre;
        private readonly Guid _userId = Guid.NewGuid();
        private GenreView _updatedGenre;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _genre = GenreDataHelper.Create("genre1");

            _updatedGenre = new GenreView {Id = _genre.Id, Name = "Some New Name"};

            Response = await GetAdminClient(_userId).PutJson($"api/genres/{_genre.Id}", _updatedGenre);

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
        public void ShouldHaveUpdateTheTranslation()
        {
            _view.Name.ShouldBe(_updatedGenre.Name);
        }
    }
}
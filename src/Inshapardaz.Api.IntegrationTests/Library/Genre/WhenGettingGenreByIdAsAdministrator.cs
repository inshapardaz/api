using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Genre
{
    [TestFixture]
    public class WhenGettingGenreByIdAsAdministrator : IntegrationTestBase
    {
        private GenreView _view;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Library.Genre _genre;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _genre = GenreDataHelper.Create("Genre2323");

            Response = await GetAdminClient(_userId).GetAsync($"/api/genres/{_genre.Id}");
            _view = JsonConvert.DeserializeObject<GenreView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_genre.Id);
        }

        [Test]
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnGenreWithCorrectId()
        {
            _view.Id.ShouldBe(_view.Id);
        }

        [Test]
        public void ShouldReturnGenreWithCorrectName()
        {
            _view.Name.ShouldBe(_genre.Name);
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test]
        public void ShouldReturnUpdateLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Update);
        }

        [Test]
        public void ShouldReturnDeleteLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Delete);
        }
    }
}
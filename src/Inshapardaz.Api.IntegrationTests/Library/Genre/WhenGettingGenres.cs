using System;
using System.Collections.Generic;
using System.Linq;
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
    public class WhenGettingGenres : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Genre> _genres = new List<Domain.Entities.Library.Genre>();
        private ListView<GenreView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _genres.Add(GenreDataHelper.Create("genre 1"));
            _genres.Add(GenreDataHelper.Create("genre 2"));
            _genres.Add(GenreDataHelper.Create("genre 3"));
            _genres.Add(GenreDataHelper.Create("genre 4"));

             Response = await GetAdminClient(Guid.NewGuid()).GetAsync($"/api/genres/");
            _view = JsonConvert.DeserializeObject<ListView<GenreView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var genre in _genres)
            {
                GenreDataHelper.Delete(genre.Id);
            }
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldContainAllGenre()
        {
            _view.Items.Count().ShouldBe(_genres.Count);
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test]
        public void ShouldReturnCreateLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Create);
        }
    }
}
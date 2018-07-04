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

namespace Inshapardaz.Api.IntegrationTests.Library.Author
{
    [TestFixture, Ignore("Security not implemented")]
    public class WhenGettingAuthorsAsReader : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Author> _genres = new List<Domain.Entities.Library.Author>();
        private ListView<AuthorView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _genres.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "genre 1" }));
            _genres.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "genre 2" }));
            _genres.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "genre 3" }));
            _genres.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "genre 4" }));

             Response = await GetReaderClient(Guid.NewGuid()).GetAsync($"/api/authors");
            _view = JsonConvert.DeserializeObject<ListView<AuthorView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var genre in _genres)
            {
                AuthorDataHelper.Delete(genre.Id);
            }
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldContainAllAuthor()
        {
            _view.Items.Count().ShouldBe(_genres.Count);
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test]
        public void ShouldNotReturnCreateLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Create);
        }
    }
}
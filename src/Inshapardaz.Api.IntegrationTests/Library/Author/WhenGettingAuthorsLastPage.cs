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
    [TestFixture]
    public class WhenGettingAuthorsLastPage : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Author> _genres = new List<Domain.Entities.Library.Author>();
        private PageView<AuthorView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _genres.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "genre 1" }));
            _genres.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "genre 2" }));
            _genres.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "genre 3" }));
            _genres.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "genre 4" }));
            _genres.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "genre 5" }));
            _genres.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "genre 6" }));

             Response = await GetAdminClient(Guid.NewGuid()).GetAsync($"/api/authors?pageNumber=3&pageSize=2");
            _view = JsonConvert.DeserializeObject<PageView<AuthorView>>(await Response.Content.ReadAsStringAsync());
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
            _view.Data.Count().ShouldBe(2);
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

        [Test]
        public void ShouldReturnPreviousLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Previous);
        }

        [Test]
        public void ShouldNotReturnNextLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Next);
        }
    }
}
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
    public class WhenGettingAuthors : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Author> _authors = new List<Domain.Entities.Library.Author>();
        private PageView<AuthorView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {

            _authors.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author 1" }));
            _authors.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author 2" }));
            _authors.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author 3" }));
            _authors.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author 4" }));

             Response = await GetAdminClient(Guid.NewGuid()).GetAsync($"/api/authors");
            _view = JsonConvert.DeserializeObject<PageView<AuthorView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var author in _authors)
            {
                AuthorDataHelper.Delete(author.Id);
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
            _view.Data.Count().ShouldBe(_authors.Count);
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
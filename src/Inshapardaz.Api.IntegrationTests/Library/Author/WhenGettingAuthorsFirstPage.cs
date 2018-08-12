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
    public class WhenGettingAuthorsFirstPage : IntegrationTestBase
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
            _authors.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author 5" }));
            _authors.Add(AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "gauthorenre 6" }));

             Response = await GetAdminClient(Guid.NewGuid()).GetAsync($"/api/authors?pageNumber=1&pageSize=2");
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
        public void ShouldNotReturnPreviousLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Previous);
        }

        [Test]
        public void ShouldReturnNextLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Next);
        }
    }
}
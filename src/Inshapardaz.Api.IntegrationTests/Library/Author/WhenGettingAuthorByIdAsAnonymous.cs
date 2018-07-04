using System;
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
    public class WhenGettingAuthorByIdAsAnonymous : IntegrationTestBase
    {
        private AuthorView _view;
        private Domain.Entities.Library.Author _author;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "Author2323" });

            Response = await GetClient().GetAsync($"/api/authors/{_author.Id}");
            _view = JsonConvert.DeserializeObject<AuthorView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            AuthorDataHelper.Delete(_author.Id);
        }

        [Test]
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnAuthorWithCorrectId()
        {
            _view.Id.ShouldBe(_view.Id);
        }

        [Test]
        public void ShouldReturnAuthorWithCorrectName()
        {
            _view.Name.ShouldBe(_author.Name);
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test]
        public void ShouldNotReturnUpdateLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Update);
        }

        [Test]
        public void ShouldNotReturnDeleteLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Delete);
        }
    }
}
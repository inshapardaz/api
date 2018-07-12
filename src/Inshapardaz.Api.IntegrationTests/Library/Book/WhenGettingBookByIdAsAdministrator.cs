using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture]
    public class WhenGettingBookByIdAsAdministrator : IntegrationTestBase
    {
        private BookView _view;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Library.Author _author;
        private Domain.Entities.Library.Book _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author1" });

            _book = new Domain.Entities.Library.Book
            {
                Title = "BookName",
                Description = "Some description",
                Language = Languages.Chinese,
                IsPublic = true,
                AuthorId = _author.Id
            };
            _book = BookDataHelper.Create(_book);

            Response = await GetAdminClient(_userId).GetAsync($"/api/books/{_book.Id}");
            _view = JsonConvert.DeserializeObject<BookView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            BookDataHelper.Delete(_book.Id);
            AuthorDataHelper.Delete(_author.Id);
        }

        [Test]
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnBookWithCorrectId()
        {
            _view.Id.ShouldBe(_view.Id);
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Self & l.Href != null);
        }

        [Test, Ignore("Security not implemented")]
        public void ShouldReturnUpdateLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Update & l.Href != null);
        }

        [Test, Ignore("Security not implemented")]
        public void ShouldReturnDeleteLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Delete & l.Href != null);
        }

        [Test]
        public void ShouldReturnAuthorLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Author & l.Href != null);
        }

        [Test]
        public void ShouldReturnCorrectTitle()
        {
            _view.Title.ShouldBe(_book.Title);
        }

        [Test]
        public void ShouldReturnCorrectDescription()
        {
            _view.Description.ShouldBe(_book.Description);
        }

        [Test]
        public void ShouldReturnCorrectPrivacy()
        {
            _view.IsPublic.ShouldBe(_book.IsPublic);
        }

        [Test]
        public void ShouldReturnCorrectLanguage()
        {
            _view.Language.ShouldBe((int)_book.Language);
        }

        [Test]
        public void ShouldReturnCorrectAuthor()
        {
            _view.AuthorId.ShouldBe(_book.AuthorId);
        }
    }
}
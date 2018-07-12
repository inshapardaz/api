using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture]
    public class WhenAddingBook : IntegrationTestBase
    {
        private BookView _view;
        private BookView _book;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Library.Author _author;
        private Domain.Entities.Library.Genre _genre;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author {Name = "author1"});
            _genre = GenreDataHelper.Create("genre1");
            _book = new BookView
            {
                Title = "BookName",
                Description = "Some description",
                LanguageId = (int)Languages.Chinese,
                IsPublic = true,
                AuthorId = _author.Id,
                Generes = new List<GenreView> { _genre.Map() }
            };

            Response = await GetAdminClient(_userId).PostJson("/api/books", _book);
            _view = JsonConvert.DeserializeObject<BookView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            AuthorDataHelper.Delete(_author.Id);
            BookDataHelper.Delete(_view.Id);
        }

        [Test]
        public void ShouldReturnCreated()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Test]
        public void ShouldReturnLocationHeader()
        {
            Response.Headers.Location.ShouldNotBeNull();
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Self & l.Href != null);
        }

        [Test]
        public void ShouldReturnUpdateLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Update & l.Href != null);
        }

        [Test]
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
            _view.LanguageId.ShouldBe(_book.LanguageId);
        }

        [Test]
        public void ShouldReturnCorrectAuthor()
        {
            _view.AuthorId.ShouldBe(_book.AuthorId);
        }

        [Test]
        public void ShouldReturnCorrectGenre()
        {
            _view.Generes.ShouldContain(g => g.Id == _genre.Id);
            _view.Generes.ShouldContain(g => g.Name == _genre.Name);
        }
    }
}
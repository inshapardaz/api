using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture]
    public class WhenUpdatingABook : IntegrationTestBase
    {
        private Domain.Entities.Library.Book _view;
        private Domain.Entities.Library.Author _author1;
        private Domain.Entities.Library.Author _author2;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Library.Book _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author1 = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author1" });
            _author2 = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author2" });

            _book = new Domain.Entities.Library.Book
            {
                Title = "BookName",
                Description = "Some description",
                Language = Languages.Chinese,
                IsPublic = true,
                AuthorId = _author1.Id
            };

            _book = BookDataHelper.Create(_book);

            _book.Title = "Some other title";
            _book.Description = "Some other Description";
            _book.Language = Languages.Arabic;
            _book.IsPublic = false;
            _book.AuthorId = _author2.Id;

            var updatedView = new BookView
            {
                Id = _book.Id,
                LanguageId = (int)_book.Language,
                Title = _book.Title,
                Description = _book.Description,
                IsPublic = _book.IsPublic,
                AuthorId = _book.AuthorId
            };

            Response = await GetAdminClient(_userId).PutJson($"api/books/{_book.Id}", updatedView);
            _view = BookDataHelper.Get(_book.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            AuthorDataHelper.Delete(_author1.Id);
            BookDataHelper.Delete(_book.Id);
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
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
            _view.Language.ShouldBe(_book.Language);
        }

        [Test]
        public void ShouldReturnCorrectAuthor()
        {
            _view.AuthorId.ShouldBe(_book.AuthorId);
        }
    }
}
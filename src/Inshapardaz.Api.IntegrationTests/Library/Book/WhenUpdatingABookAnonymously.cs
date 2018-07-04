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
    public class WhenUpdatingABookAnonymously : IntegrationTestBase
    {
        private Domain.Entities.Library.Book _book;
        private Domain.Entities.Library.Author _author;

        private BookView _updatedBook;

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

            _updatedBook = new BookView
            {
                Id = _book.Id,
                LanguageId = (int)_book.Language,
                Title = _book.Title,
                Description = _book.Description,
                IsPublic = _book.IsPublic,
                AuthorId = _book.AuthorId
            };

            Response = await GetClient().PutJson($"api/books/{_book.Id}", _updatedBook);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            AuthorDataHelper.Delete(_author.Id);
            BookDataHelper.Delete(_book.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture]
    public class WhenDeletingAnBook : IntegrationTestBase
    {
        private Domain.Entities.Library.Book _view;
        private Domain.Entities.Library.Author _author;
        private readonly Guid _userId = Guid.NewGuid();
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

            Response = await GetAdminClient(_userId).DeleteAsync($"api/books/{_book.Id}");

            _view = BookDataHelper.Get(_book.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            AuthorDataHelper.Delete(_author.Id);
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Test]
        public void ShouldHaveDeletedTheBook()
        {
            _view.ShouldBeNull();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture]
    public class WhenGettingFavoriteBooks : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Book> _books = new List<Domain.Entities.Library.Book>();
        private readonly List<Domain.Entities.Library.Book> _favorites = new List<Domain.Entities.Library.Book>();
        private readonly Guid UserId = Guid.NewGuid();

        private Page<BookView> _view;
        private Domain.Entities.Library.Author _author;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author1" });

            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 1", AuthorId = _author.Id, DateAdded = DateTime.Today.AddDays(-4)}));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 2", AuthorId = _author.Id, DateAdded = DateTime.Today.AddDays(-2) }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 3", AuthorId = _author.Id, DateAdded = DateTime.Today.AddDays(-3) }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 4", AuthorId = _author.Id, DateAdded = DateTime.Today.AddDays(-1) }));

            _favorites.Add(BookDataHelper.AddToFavorites(UserId, _books[0]));
            _favorites.Add(BookDataHelper.AddToFavorites(UserId, _books[3]));

            Response = await GetReaderClient(UserId).GetAsync($"/api/books/favorite");
            _view = JsonConvert.DeserializeObject<Page<BookView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var book in _books)
            {
                BookDataHelper.Delete(book.Id);
            }

            foreach (var book in _favorites)
            {
                BookDataHelper.RemoveFromFavorites(UserId, book);
            }

            AuthorDataHelper.Delete(_author.Id);
        }

        [Test]
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldContainAllBook()
        {
            _view.Data.Count().ShouldBe(_favorites.Count);
        }

        [Test]
        public void ShouldReturnBooksFromNewestToOldest()
        {
            _view.Data.ShouldContain(b => b.Id == _books[0].Id);
            _view.Data.ShouldContain(b => b.Id == _books[3].Id);
        }
    }
}
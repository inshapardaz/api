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

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture]
    public class WhenGettingLatestBooks : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Book> _books = new List<Domain.Entities.Library.Book>();
        private IEnumerable<BookView> _view;
        private Domain.Entities.Library.Author _author;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author1" });

            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 1", AuthorId = _author.Id, DateAdded = DateTime.Today.AddDays(-4)}));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 2", AuthorId = _author.Id, DateAdded = DateTime.Today.AddDays(-2) }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 3", AuthorId = _author.Id, DateAdded = DateTime.Today.AddDays(-3) }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 4", AuthorId = _author.Id, DateAdded = DateTime.Today.AddDays(-1) }));

             Response = await GetAdminClient(Guid.NewGuid()).GetAsync($"/api/books/latest");
            _view = JsonConvert.DeserializeObject<IEnumerable<BookView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var book in _books)
            {
                BookDataHelper.Delete(book.Id);
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
            _view.Count().ShouldBe(_books.Count);
        }

        [Test]
        public void ShouldReturnBooksFromNewestToOldest()
        {
            _view.ElementAt(0).Id = _books[3].Id;
            _view.ElementAt(1).Id = _books[1].Id;
            _view.ElementAt(2).Id = _books[2].Id;
            _view.ElementAt(3).Id = _books[0].Id;
        }
    }
}
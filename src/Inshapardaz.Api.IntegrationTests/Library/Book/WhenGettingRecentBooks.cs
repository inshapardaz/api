using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View.Library;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture]
    public class WhenGettingRecentBooks : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Book> _books = new List<Domain.Entities.Library.Book>();
        private readonly List<Domain.Entities.Library.Book> _recents = new List<Domain.Entities.Library.Book>();

        private IEnumerable<BookView> _view;
        private Domain.Entities.Library.Author _author;
        private Guid UserId = Guid.NewGuid();
        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author1" });

            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 1", AuthorId = _author.Id }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 2", AuthorId = _author.Id }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 3", AuthorId = _author.Id }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 4", AuthorId = _author.Id }));

            _recents.Add(BookDataHelper.AddRecent(UserId, _books[0]));
            _recents.Add(BookDataHelper.AddRecent(UserId, _books[3]));
            
            Response = await GetReaderClient(UserId).GetAsync("/api/books/recent");
            _view = JsonConvert.DeserializeObject<IEnumerable<BookView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var book in _books)
            {
                BookDataHelper.Delete(book.Id);
            }

            foreach (var book in _recents)
            {
                BookDataHelper.RemoveFromRecent(UserId, book);
            }

            AuthorDataHelper.Delete(_author.Id);
        }

        [Test]
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldContainAllRecentBook()
        {
            _view.Count().ShouldBe(_recents.Count);
            foreach (var book in _recents)
            {
                Assert.That(_view.Any(b => b.Id == book.Id), $"Expected book ${book.Id} to be in recents");
            }
        }
    }
}
using System.Threading;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class BookDataHelper
    {
        private readonly IBookRepository _bookRepository;

        public BookDataHelper(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public Book Create(Book book)
        {
            return _bookRepository.AddBook(book , CancellationToken.None).Result;
        }

        public Book Get(int bookId)
        {
            return _bookRepository.GetBookById(bookId, CancellationToken.None).Result;
        }

        public void Delete(int bookId)
        {
            _bookRepository.DeleteBook(bookId, CancellationToken.None).Wait();
        }
    }
}
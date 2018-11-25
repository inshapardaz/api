using System;
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

        public Book AddRecent(Guid userId, Book book)
        {
            _bookRepository.AddRecentBook(userId, book.Id, CancellationToken.None).Wait();
            return book;
        }

        public void RemoveFromRecent(Guid userId, Book book)
        {
            _bookRepository.DeleteBookFromRecent(userId, book.Id, CancellationToken.None).Wait();
        }

        public Book AddToFavorites(Guid userId, Book book)
        {
            _bookRepository.AddBookToFavorites(userId, book.Id, CancellationToken.None).Wait();
            return book;
        }

        public void RemoveFromFavorites(Guid userId, Book book)
        {
            _bookRepository.DeleteBookFromFavorites(userId, book.Id, CancellationToken.None).Wait();
        }
    }
}
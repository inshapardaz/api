using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class BookRepository : IBookRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public BookRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Book> AddBook(Book book, CancellationToken cancellationToken)
        {
            var author = await _databaseContext.Author
                                             .SingleOrDefaultAsync(t => t.Id == book.AuthorId,
                                                                   cancellationToken);
            if (author == null)
            {
                throw new NotFoundException();
            }

            var item = book.Map<Book, Entities.Library.Book>();
            author.Books.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return item.Map<Entities.Library.Book, Book>();
        }

        public async Task UpdateBook(Book book, CancellationToken cancellationToken)
        {
            var existingEntity = await _databaseContext.Book
                                                       .SingleOrDefaultAsync(g => g.Id == book.Id,
                                                                             cancellationToken);

            if (existingEntity == null)
            {
                throw new NotFoundException();
            }

            existingEntity.Title = book.Title;
            existingEntity.Description = book.Description;
            existingEntity.AuthorId = book.AuthorId;
            existingEntity.IsPublic = book.IsPublic;
            existingEntity.Language = book.Language;

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteBook(int bookId, CancellationToken cancellationToken)
        {
            var book = await _databaseContext.Book.SingleOrDefaultAsync(g => g.Id == bookId, cancellationToken);

            if (book == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.Book.Remove(book);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Page<Book>> GetBooks(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext.Book;

            var count = book.Count();
            var data = await book
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map<Entities.Library.Book, Book>())
                             .ToListAsync(cancellationToken);

            return new Page<Book>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<Page<Book>> GetBooksByGenere(int genereId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext.Book;

            var count = book.Count();
            var data = await book
                             .Where(b => b.Generes.Any(g => g.GenereId == genereId))
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map<Entities.Library.Book, Book>())
                             .ToListAsync(cancellationToken);

            return new Page<Book>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<Page<Book>> GetBooksByAuthor(int authorId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext.Book;

            var count = book.Count();
            var data = await book
                             .Where(b => b.AuthorId == authorId)
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map<Entities.Library.Book, Book>())
                             .ToListAsync(cancellationToken);

            return new Page<Book>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<Book> GetBookById(int bookId, CancellationToken cancellationToken)
        {
            var book = await _databaseContext.Book
                                               .SingleOrDefaultAsync(t => t.Id == bookId,
                                                                     cancellationToken);
            return book.Map<Entities.Library.Book, Book>();
        }
    }
}
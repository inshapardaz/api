using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Inshapardaz.Ports.Database.Entities.Library;
using Microsoft.EntityFrameworkCore;
using Book = Inshapardaz.Domain.Entities.Library.Book;

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
            item.Generes.Clear();
            item.Author = author;

            _databaseContext.Book.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            item.Generes = new List<BookGenre>();
            foreach (var genre in book.Generes)
            {
                var gen = await _databaseContext.Genere.SingleOrDefaultAsync(g => g.Id == genre.Id, cancellationToken);
                if (gen != null)
                    item.Generes.Add(new BookGenre { BookId = item.Id, Book = item, GenreId = gen.Id, Genre = gen});
            }
            
            await _databaseContext.SaveChangesAsync(cancellationToken);

            var newBook = await _databaseContext.Book
                                                .Include(b => b.Author)
                                                .Include(b => b.Generes)
                                                .SingleOrDefaultAsync(t => t.Id == item.Id,
                                                                      cancellationToken);
            return newBook.Map<Entities.Library.Book, Book>();
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
                             .Include(b => b.Author)
                             .Include(b => b.Generes)
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

        public async Task<Page<Book>> SearchBooks(string searchText, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _databaseContext.Book
                             .Include(b => b.Author)
                             .Include(b => b.Generes)
                             .Where(b => b.Title.Contains(searchText));
            var count = query.Count();
            var data = await query
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
            var book = _databaseContext.Book
                                       .Where(b => b.Generes.Any(g => g.GenreId == genereId));

            var count = book.Count();
            var data = await book
                             .Include(b => b.Author)
                             .Include(b => b.Generes)
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
            var book = _databaseContext
                        .Book
                        .Where(b => b.AuthorId == authorId);

            var count = book.Count();
            var data = await book
                             .Include(b => b.Author)
                             .Include(b => b.Generes)
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

        public async Task<Page<Book>> GetPublicBooksByAuthor(int authorId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext
                       .Book
                       .Where(b => b.AuthorId == authorId && b.IsPublic);

            var count = book.Count();
            var data = await book
                             .Include(b => b.Author)
                             .Include(b => b.Generes)
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

        public async Task<Page<Book>> GetPublicBooks(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext.Book.Where(b => b.IsPublic);

            var count = book.Count();
            var data = await book
                             .Include(b => b.Author)
                             .Include(b => b.Generes)
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

        public async Task<Page<Book>> SearchPublicBooks(string searchText, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _databaseContext.Book
                             .Include(b => b.Author)
                             .Include(b => b.Generes)
                             .Where(b => b.IsPublic && b.Title.Contains(searchText));
            var count = query.Count();
            var data = await  query
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
                                             .Include(b => b.Author)
                                             .Include(b => b.Generes)
                                             .SingleOrDefaultAsync(t => t.Id == bookId,
                                                                     cancellationToken);
            return book.Map<Entities.Library.Book, Book>();
        }
    }
}
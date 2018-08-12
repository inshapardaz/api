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
            item.BookCategory.Clear();
            item.Author = author;

            _databaseContext.Book.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            item.BookCategory = new List<BookCategory>();
            foreach (var category in book.Categories)
            {
                var gen = await _databaseContext.Category.SingleOrDefaultAsync(g => g.Id == category.Id, cancellationToken);
                if (gen != null)
                    item.BookCategory.Add(new BookCategory { BookId = item.Id, Book = item, CategoryId = gen.Id, Category = gen});
            }
            
            await _databaseContext.SaveChangesAsync(cancellationToken);

            var newBook = await _databaseContext.Book
                                                .Include(b => b.Author)
                                                .Include(b => b.BookCategory)
                                                .ThenInclude(c => c.Category)
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
            if (book.ImageId > 0)
            {
                existingEntity.ImageId = book.ImageId;
            }

            existingEntity.BookCategory.Clear();

            await _databaseContext.SaveChangesAsync(cancellationToken);

            foreach (var category in book.Categories)
            {
                var gen = await _databaseContext.Category.SingleOrDefaultAsync(g => g.Id == category.Id, cancellationToken);
                if (gen != null)
                    existingEntity.BookCategory.Add(new BookCategory { BookId = existingEntity.Id, Book = existingEntity, CategoryId = gen.Id, Category = gen });
            }

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
            var book = _databaseContext.Book
                        .Include(b => b.Author)
                        .Include(b => b.BookCategory)
                        .ThenInclude(c => c.Category);

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

        public async Task<Page<Book>> SearchBooks(string searchText, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _databaseContext.Book
                            .Include(b => b.Author)
                            .Include(b => b.BookCategory)
                            .ThenInclude(c => c.Category)
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

        public async Task<Page<Book>> GetBooksByCategory(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext.Book
                        .Include(b => b.Author)
                        .Include(b => b.BookCategory)
                        .ThenInclude(c => c.Category)
                        .Where(b => b.BookCategory.Any(g => g.CategoryId == categoryId));

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

        public async Task<Page<Book>> GetPublicBooksByCategory(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext.Book
                        .Include(b => b.Author)
                        .Include(b => b.BookCategory)
                        .ThenInclude(c => c.Category)
                        .Where(b => b.BookCategory.Any(g => g.CategoryId == categoryId) && b.IsPublic);

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

        public async Task<Page<Book>> GetBooksByAuthor(int authorId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext
                        .Book
                        .Include(b => b.Author)
                        .Include(b => b.BookCategory)
                        .ThenInclude(c => c.Category)
                        .Where(b => b.AuthorId == authorId);

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

        public async Task<Page<Book>> GetPublicBooksByAuthor(int authorId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext
                        .Book
                        .Include(b => b.Author)
                        .Include(b => b.BookCategory)
                        .ThenInclude(c => c.Category)
                        .Where(b => b.AuthorId == authorId && b.IsPublic);

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

        public async Task<Page<Book>> GetPublicBooks(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext.Book.Where(b => b.IsPublic);

            var count = book.Count();
            var data = await book
                             .Include(b => b.Author)
                             .Include(b => b.BookCategory)
                             .ThenInclude(c => c.Category)
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
                             .Include(b => b.BookCategory)
                             .ThenInclude(c => c.Category)
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
                                             .Include(b => b.BookCategory)
                                             .ThenInclude(c => c.Category)
                                             .SingleOrDefaultAsync(t => t.Id == bookId,
                                                                     cancellationToken);
            return book.Map<Entities.Library.Book, Book>();
        }
    }
}
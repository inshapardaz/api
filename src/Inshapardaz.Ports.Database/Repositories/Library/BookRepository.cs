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

            var series = await _databaseContext.Series.SingleOrDefaultAsync(s => s.Id == book.SeriesId, cancellationToken);

            var item = book.Map();
            item.BookCategory.Clear();
            item.Series = series;
            item.Author = author;

            _databaseContext.Book.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            item.BookCategory = new List<BookCategory>();

            if (book.Categories != null)
            {
                foreach (var category in book.Categories)
                {
                    var gen = await _databaseContext.Category.SingleOrDefaultAsync(g => g.Id == category.Id, cancellationToken);
                    if (gen != null)
                        item.BookCategory.Add(new BookCategory {BookId = item.Id, Book = item, CategoryId = gen.Id, Category = gen});
                }
            }
            
            await _databaseContext.SaveChangesAsync(cancellationToken);

            var newBook = await _databaseContext.Book
                                                .Include(b => b.Author)
                                                .Include(b => b.Series)
                                                .Include(b => b.BookCategory)
                                                .ThenInclude(c => c.Category)
                                                .SingleOrDefaultAsync(t => t.Id == item.Id,
                                                                      cancellationToken);
            return newBook.Map();
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
            existingEntity.SeriesId = book.SeriesId;
            existingEntity.SeriesIndex = book.SeriesIndex;
            existingEntity.Status = book.Status;
            existingEntity.Copyrights = book.Copyrights;
            existingEntity.YearPublished = book.YearPublished;
            existingEntity.IsPublished = book.IsPublished;

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

            if (book != null)
            {
                _databaseContext.Book.Remove(book);
            }

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Page<Book>> GetBooks(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext.Book
                        .Include(b => b.Author)
                        .Include(b => b.Series)
                        .Include(b => b.BookCategory)
                        .ThenInclude(c => c.Category);

            var count = await book.CountAsync(cancellationToken);
            var data = await book
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map())
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
                            .Include(b => b.Series)
                            .Include(b => b.BookCategory)
                            .ThenInclude(c => c.Category)
                            .Where(b => b.Title.Contains(searchText));
            var count = await query.CountAsync(cancellationToken);
            var data = await query
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map())
                             .ToListAsync(cancellationToken);

            return new Page<Book>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<IEnumerable<Book>> GetLatestBooks(CancellationToken cancellationToken)
        {
            return await _databaseContext.Book
                                        .Include(b => b.Author)
                                        .Include(b => b.Series)
                                        .Include(b => b.BookCategory)
                                        .ThenInclude(c => c.Category)
                                        .OrderByDescending(b => b.DateAdded)
                                        .Take(10)
                                        .Select(a => a.Map())
                                        .ToListAsync(cancellationToken);
        }

        public async Task<Page<Book>> GetBooksByCategory(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext.Book
                        .Include(b => b.Author)
                        .Include(b => b.Series)
                        .Include(b => b.BookCategory)
                        .ThenInclude(c => c.Category)
                        .Where(b => b.BookCategory.Any(g => g.CategoryId == categoryId));

            var count = book.Count();
            var data = await book
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map())
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
                        .Include(b => b.Series)
                        .Include(b => b.BookCategory)
                        .ThenInclude(c => c.Category)
                        .Where(b => b.AuthorId == authorId);

            var count = book.Count();
            var data = await book
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map())
                             .ToListAsync(cancellationToken);

            return new Page<Book>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<Page<Book>> GetBooksBySeries(int seriesId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var book = _databaseContext
                       .Book
                       .Include(b => b.Author)
                       .Include(b => b.Series)
                       .Include(b => b.BookCategory)
                       .ThenInclude(c => c.Category)
                       .Where(b => b.SeriesId == seriesId)
                       .OrderBy(b => b.SeriesIndex);

            var count = book.Count();
            var data = await book
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map())
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
                                             .Include(b => b.Series)
                                             .Include(b => b.BookCategory)
                                             .ThenInclude(c => c.Category)
                                             .SingleOrDefaultAsync(t => t.Id == bookId,
                                                                     cancellationToken);
            return book.Map();
        }

        public async Task AddRecentBook(Guid userId, int bookId, CancellationToken cancellationToken)
        {
            var book = await _databaseContext.Book.SingleOrDefaultAsync(b => b.Id == bookId, cancellationToken);
            if (book == null)
            {
                throw new NotFoundException();
            }

            var recent = await _databaseContext.RecentBooks.SingleOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId, cancellationToken);

            if (recent == null)
            {
                recent = new RecentBook
                {
                    UserId = userId,
                    BookId = bookId,
                    DateRead = DateTime.UtcNow,
                    Book = book
                };
                await _databaseContext.RecentBooks.AddAsync(recent, cancellationToken);
            }
            else
            {
                recent.DateRead = DateTime.UtcNow;
            }
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteBookFromRecent(Guid userId, int bookId, CancellationToken cancellationToken)
        {
            var recent = await _databaseContext.RecentBooks.SingleOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId, cancellationToken);

            if (recent == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.RecentBooks.Remove(recent);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Book>> GetRecentBooksByUser(Guid userId, int count, CancellationToken cancellationToken)
        {
            var recents = await _databaseContext.RecentBooks
                                                .Include(r => r.Book)
                                                .Where(r => r.UserId == userId)
                                                .OrderByDescending(r => r.DateRead)
                                                .Take(count)
                                                .Select(r => r.Book)
                                                .ToListAsync(cancellationToken);
            return recents.Select(b => b.Map());
        }

        public async Task AddBookToFavorites(Guid userId, int bookId, CancellationToken cancellationToken)
        {
            var book = await _databaseContext.Book.SingleOrDefaultAsync(b => b.Id == bookId, cancellationToken);
            if (book == null)
            {
                throw new NotFoundException();
            }

            var fav = await _databaseContext.FavoriteBooks.SingleOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId, cancellationToken);

            if (fav == null)
            {
                fav = new FavoriteBook
                {
                    UserId = userId,
                    BookId = bookId,
                    DateAdded = DateTime.UtcNow,
                    Book = book
                };
                await _databaseContext.FavoriteBooks.AddAsync(fav, cancellationToken);
                await _databaseContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteBookFromFavorites(Guid userId, int bookId, CancellationToken cancellationToken)
        {
            var fav = await _databaseContext
                                .FavoriteBooks
                                .SingleOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId, cancellationToken);

            if (fav == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.FavoriteBooks.Remove(fav);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Page<Book>> GetFavoriteBooksByUser(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _databaseContext.FavoriteBooks
                                                .Include(r => r.Book)
                                                .Where(r => r.UserId == userId)
                                                .OrderBy(r => r.Book.Title);
            var count = await query.CountAsync(cancellationToken);
            var data = await query.Paginate(pageNumber, pageSize)
                                                .Select(r => r.Book)
                                                .Select(a => a.Map())
                                                .ToListAsync(cancellationToken);

            return new Page<Book>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<int> GetBookCountByAuthor(int authorId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Book.CountAsync(b => b.AuthorId == authorId, cancellationToken);
        }

        public async Task<int> GetBookCountBySeries(int seriesId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Book.CountAsync(b => b.SeriesId == seriesId, cancellationToken);
        }

        public async Task<int> GetBookCountByCategory(int categoryId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Book.CountAsync(b => b.BookCategory.Any(g => g.CategoryId == categoryId), cancellationToken);
        }

        public async Task DeleteBookFile(int bookId, int fileId, CancellationToken cancellationToken)
        {
            var bookFile = await _databaseContext.BookFiles.SingleOrDefaultAsync(bf => bf.Id == fileId, cancellationToken: cancellationToken);

            if (bookFile != null)
            {
                _databaseContext.BookFiles.Remove(bookFile);
                await _databaseContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<File> GetBookFileById(int bookId, int fileId, CancellationToken cancellationToken)
        {
            var bookFile = await _databaseContext.BookFiles
                                                 .Include(b=> b.File)
                                                 .SingleOrDefaultAsync(bf => bf.Id == fileId, cancellationToken: cancellationToken);

            return bookFile?.File.Map();
        }

        public async Task<IEnumerable<File>> GetFilesByBook(int bookId, CancellationToken cancellationToken)
        {
            var files = await _databaseContext.BookFiles
                                              .Include(bf => bf.File)
                                              .Where(f => f.BookId == bookId)
                                              .ToListAsync(cancellationToken);
            return files.Select(bf => bf.File.Map());
        }

        public async Task AddBookFile(int bookId, int fileId, CancellationToken cancellationToken)
        {
            _databaseContext.BookFiles.Add(new BookFile {BookId = bookId, FileId = fileId });
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }
    }
}

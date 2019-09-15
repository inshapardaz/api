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
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public FavoriteRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
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

        public async Task<bool> IsBookFavorite(Guid userId, int bookId, CancellationToken cancellationToken)
        {
            return await _databaseContext.FavoriteBooks.AnyAsync(f => f.BookId == bookId && f.UserId == userId, cancellationToken);
        }
    }
}

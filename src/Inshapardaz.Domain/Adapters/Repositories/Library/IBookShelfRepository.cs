﻿using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library;

public interface IBookShelfRepository
{
    Task<BookShelfModel> AddBookShelf(int libraryId, BookShelfModel bookShelf, CancellationToken cancellationToken);

    Task UpdateBookShelf(int libraryId, BookShelfModel bookShelf, CancellationToken cancellationToken);

    Task DeleteBookShelf(int libraryId, int bookshelfId, CancellationToken cancellationToken);

    Task<Page<BookShelfModel>> GetBookShelves(int libraryId, bool onlyPublic, int pageNumber, int pageSize, int? accountId, CancellationToken cancellationToken);

    Task<Page<BookShelfModel>> FindBookShelves(int libraryId, string query, bool onlyPublic, int pageNumber, int pageSize, int? accountId, CancellationToken cancellationToken);

    Task<BookShelfModel> GetBookShelfById(int libraryId, int bookshelfId, CancellationToken cancellationToken);

    Task UpdateBookShelfImage(int libraryId, int bookshelfId, long imageId, CancellationToken cancellationToken);

    Task AddBookToBookShelf(int libraryId, int bookshelfId, int bookId, int index, CancellationToken cancellationToken);

    Task UpdateBookToBookShelf(int libraryId, BookShelfBook bookShelfBook, CancellationToken cancellationToken);

    Task RemoveBookFromBookShelf(int libraryId, int bookshelfId, int bookId, CancellationToken cancellationToken);
    Task<BookShelfBook> GetBookFromBookShelfById(int libraryId, int bookShelfId, int bookId, CancellationToken cancellationToken);

    #region for Migration
    Task<Page<BookShelfModel>> GetAllBookShelves(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<BookShelfBook>> GetBookShelfBooks(int libraryId, int bookShelfId, CancellationToken cancellationToken);
    #endregion region
}

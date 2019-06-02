﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IBookRepository
    {
        Task<Book> AddBook(Book book, CancellationToken cancellationToken);

        Task UpdateBook(Book book, CancellationToken cancellationToken);

        Task DeleteBook(int bookId, CancellationToken cancellationToken);

        Task<Page<Book>> GetBooks(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<Book>> GetLatestBooks(CancellationToken cancellationToken);
        
        Task<Page<Book>> SearchBooks(string searchText, int pageNumber, int pageSize, CancellationToken cancellationToken);


        Task<Page<Book>> GetBooksByAuthor(int authorId, int pageNumber, int pageSize, CancellationToken cancellationToken);


        Task<Page<Book>> GetBooksByCategory(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Book> GetBookById(int bookId, CancellationToken cancellationToken);


        Task AddRecentBook(Guid userId, int bookId, CancellationToken cancellationToken);

        Task DeleteBookFromRecent(Guid userId, int bookId, CancellationToken cancellationToken);

        Task<IEnumerable<Book>> GetRecentBooksByUser(Guid userId, int count, CancellationToken cancellationToken);

        Task AddBookToFavorites(Guid userId, int bookId, CancellationToken cancellationToken);

        Task DeleteBookFromFavorites(Guid userId, int bookId, CancellationToken cancellationToken);

        Task<Page<Book>> GetFavoriteBooksByUser(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task AddBookFile(int bookId, int id, CancellationToken cancellationToken);
        Task<int> GetBookCountByAuthor(int authorId, CancellationToken cancellationToken);
        Task<Page<Book>> GetBooksBySeries(int seriesId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<int> GetBookCountBySeries(int seriesId, CancellationToken cancellationToken);
        Task<int> GetBookCountByCategory(int categoryId, CancellationToken cancellationToken);
        Task DeleteBookFile(int bookId, int fileId, CancellationToken cancellationToken);

        Task<IEnumerable<File>> GetFilesByBook(int bookId, CancellationToken cancellationToken);

        Task<File> GetBookFileById(int bookId, int fileId, CancellationToken cancellationToken);
    }
}
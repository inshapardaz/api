﻿using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public AuthorRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<AuthorModel> AddAuthor(int libraryId, AuthorModel author, CancellationToken cancellationToken)
        {
            int authorId;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into Author(Name, Description, ImageId, LibraryId) OUTPUT Inserted.Id VALUES(@Name, @Description, @ImageId, @LibraryId);";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Name = author.Name, Description = author.Description, ImageId = author.ImageId }, cancellationToken: cancellationToken);
                authorId = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetAuthorById(libraryId, authorId, cancellationToken);
        }

        public async Task UpdateAuthor(int libraryId, AuthorModel author, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Author Set Name = @Name, Description = @Description, ImageId = @ImageId Where Id = @Id AND LibraryId = @LibraryId";
                var command = new CommandDefinition(sql, new { Id = author.Id, LibraryId = libraryId, Name = author.Name, Description = author.Description, ImageId = author.ImageId }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task UpdateAuthorImage(int libraryId, int authorId, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Author Set ImageId = @ImageId Where Id = @Id AND LibraryId = @LibraryId ";
                var command = new CommandDefinition(sql, new { Id = authorId, LibraryId = libraryId, ImageId = imageId }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteAuthor(int libraryId, int authorId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Author Where LibraryId = @LibraryId AND Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = authorId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<Page<AuthorModel>> GetAuthors(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT a.Id, a.Name, a.Description, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*)
                                FROM Book b
                                INNER JOIN BookAuthor ba ON ba.BookId = b.Id
                                WHERE ba.AuthorId = a.Id  AND b.Status = 0) AS BookCount
                            FROM Author AS a
                            LEFT OUTER JOIN [File] f ON f.Id = a.ImageId
                            Where a.LibraryId = @LibraryId
                            Order By a.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var authors = await connection.QueryAsync<AuthorModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Author WHERE LibraryId = @LibraryId";
                var authorCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId }, cancellationToken: cancellationToken));

                return new Page<AuthorModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = authorCount,
                    Data = authors
                };
            }
        }

        public async Task<AuthorModel> GetAuthorById(int libraryId, int authorId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT a.Id, a.Name, a.Description, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*)
                                FROM Book b
                                INNER JOIN BookAuthor ba ON ba.BookId = b.Id
                                WHERE ba.AuthorId = a.Id  AND b.Status = 0) AS BookCount
                            FROM Author AS a
                            LEFT OUTER JOIN [File] f ON f.Id = a.ImageId
                            Where a.LibraryId = @LibraryId
                            And a.Id = @AuthorId";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, AuthorId = authorId },
                                                    cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<AuthorModel>(command);
            }
        }

        public async Task<Page<AuthorModel>> FindAuthors(int libraryId, string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT a.Id, a.Name, a.Description, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*)
                                FROM Book b
                                INNER JOIN BookAuthor ba ON ba.BookId = b.Id
                                WHERE ba.AuthorId = a.Id  AND b.Status = 0) AS BookCount
                            FROM Author AS a
                            LEFT OUTER JOIN [File] f ON f.Id = a.ImageId
                            Where a.LibraryId = @LibraryId AND
                            a.Name LIKE @Query
                            Order By a.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, Query = $"%{query}%", PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var authors = await connection.QueryAsync<AuthorModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Author WHERE LibraryId = @LibraryId And Name LIKE @Query";
                var authorCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId, Query = $"%{query}%" }, cancellationToken: cancellationToken));

                return new Page<AuthorModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = authorCount,
                    Data = authors
                };
            }
        }

        public async Task<IEnumerable<AuthorModel>> GetAuthorByIds(int libraryId, IEnumerable<int> authorIds, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT a.Id, a.Name, a.Description, f.Id As ImageId, f.FilePath AS ImageUrl,
                            (SELECT Count(*)
                                FROM Book b
                                INNER JOIN BookAuthor ba ON ba.BookId = b.Id
                                WHERE ba.AuthorId = a.Id  AND b.Status = 0) AS BookCount
                            FROM Author AS a
                            LEFT OUTER JOIN [File] f ON f.Id = a.ImageId
                            Where a.LibraryId = @LibraryId
                            And a.Id IN @AuthorIds";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, AuthorIds = authorIds },
                                                    cancellationToken: cancellationToken);

                return await connection.QueryAsync<AuthorModel>(command);
            }
        }
    }
}

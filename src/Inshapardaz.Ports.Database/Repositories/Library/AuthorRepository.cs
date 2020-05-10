using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Database.Repositories.Library
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
                var sql = @"Insert Into Library.Author(Name, ImageId, LibraryId) OUTPUT Inserted.Id VALUES(@Name, @ImageId, @LibraryId);";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Name = author.Name, ImageId = author.ImageId }, cancellationToken: cancellationToken);
                authorId = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetAuthorById(libraryId, authorId, cancellationToken);
        }

        public async Task UpdateAuthor(int libraryId, AuthorModel author, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Library.Author Set Name = @Name, ImageId = @ImageId Where Id = @Id AND LibraryId = @LibraryId";
                var command = new CommandDefinition(sql, new { Id = author.Id, LibraryId = libraryId, Name = author.Name, ImageId = author.ImageId }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task UpdateAuthorImage(int libraryId, int authorId, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Library.Author Set ImageId = @ImageId Where Id = @Id AND LibraryId = @LibraryId ";
                var command = new CommandDefinition(sql, new { Id = authorId, LibraryId = libraryId, ImageId = imageId }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteAuthor(int libraryId, int authorId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Library.Author Where LibraryId = @LibraryId AND Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = authorId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<Page<AuthorModel>> GetAuthors(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT a.Id, a.Name, f.Id As ImageId, f.FilePath AS ImageUrl, (SELECT Count(*) FROM Library.Book b WHERE b.AuthorId = a.Id) AS BookCount
                            FROM Library.Author AS a
                            INNER JOIN Inshapardaz.[File] f ON f.Id = a.ImageId
                            Where a.LibraryId = @LibraryId
                            Order By a.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var authors = await connection.QueryAsync<AuthorModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Library.Author WHERE LibraryId = @LibraryId";
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
                var sql = @"SELECT a.Id, a.Name, f.Id As ImageId, f.FilePath AS ImageUrl, (SELECT Count(*) FROM Library.Book b WHERE b.AuthorId = a.Id) AS BookCount
                            FROM Library.Author AS a
                            LEFT OUTER JOIN Inshapardaz.[File] f ON f.Id = a.ImageId
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
                var sql = @"SELECT a.Id, a.Name, f.Id As ImageId, f.FilePath AS ImageUrl, (SELECT Count(*) FROM Library.Book b WHERE b.AuthorId = a.Id) AS BookCount
                            FROM Library.Author AS a
                            INNER JOIN Inshapardaz.[File] f ON f.Id = a.ImageId
                            Where a.LibraryId = @LibraryId AND
                            a.Name LIKE @Query
                            Order By a.Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, Query = $"%{query}%", PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var authors = await connection.QueryAsync<AuthorModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Library.Author WHERE LibraryId = @LibraryId And Name LIKE @Query";
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
    }
}

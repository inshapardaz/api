using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class BookPageRepository : IBookPageRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public BookPageRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<BookPageModel> AddPage(int libraryId, int bookId, int pageNumber, string text, int imageId, CancellationToken cancellationToken)
        {
            int authorId;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into BookPage(BookId, PageNumber, Text, ImageId) OUTPUT Inserted.Id VALUES(@BookId, @PageNumber, @Text, @ImageId);";
                var command = new CommandDefinition(sql, new { BookId = bookId, PageNumber = pageNumber, Text = text, ImageId = imageId }, cancellationToken: cancellationToken);
                authorId = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetPageByPageNumber(libraryId, bookId, pageNumber, cancellationToken);
        }

        public async Task<BookPageModel> GetPageByPageNumber(int libraryId, int bookId, int pageNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT p.BookId, p.PageNumber, p.Text, f.Id As ImageId
                            FROM BookPage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            Where p.BookId = @BookId AND p.PageNumber = @PageNumber";
                var command = new CommandDefinition(sql,
                                                    new { BookId = bookId, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<BookPageModel>(command);
            }
        }

        public async Task DeletePage(int libraryId, int bookId, int pageNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From BookPage Where BookId = @BookId AND PageNumber = @PageNumber";
                var command = new CommandDefinition(sql, new { BookId = bookId, PageNumber = pageNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<BookPageModel> UpdatePage(int libraryId, int bookId, int pageNumber, string text, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update BookPage Set Text = @Text, ImageId = @ImageId Where BookId = @BookId AND PageNumber = @PageNumber";
                var command = new CommandDefinition(sql, new { Text = text, ImageId = imageId, BookId = bookId, PageNumber = pageNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetPageByPageNumber(libraryId, bookId, pageNumber, cancellationToken);
            }
        }

        public async Task<BookPageModel> UpdatePageImage(int libraryId, int bookId, int pageNumber, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update BookPage Set ImageId = @ImageId Where BookId = @BookId AND PageNumber = @PageNumber";
                var command = new CommandDefinition(sql, new { ImageId = imageId, BookId = bookId, PageNumber = pageNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetPageByPageNumber(libraryId, bookId, pageNumber, cancellationToken);
            }
        }
    }
}

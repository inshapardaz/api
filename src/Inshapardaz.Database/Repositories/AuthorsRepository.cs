using Inshapardaz.Database;
using Inshapardaz.Database.Dto.Library;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.Repositories
{
    public class AuthorsRepository : BaseRepository, IAuthorRepository
    {
        public Task<Author> AddAuthor(Author author, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAuthor(int authorId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Page<Author>> FindAuthors(string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var countQuery = @"SELECT * FROM Authors WHERE Name LIKE @Name";
           
            var totalCount = await ExecuteScalar<int>(countQuery, new { Name = query });

            var sql = @"SELECT * FROM Authors 
                        WHERE Name LIKE @Name
                        OFFSET @PageNumber*@RowsPerPage ROWS
                        FETCH NEXT @RowsPerPage ROWS ONLY";

            var parameter = new
            {
                Name = query,
                PageNumber = pageNumber,
                RowsPerPage = pageSize
            };

            var authors = await QueryAsync<AuthorDto>(sql, parameter);

            return authors.Map(pageNumber, pageSize, totalCount);
        }

        public Task<Author> GetAuthorById(int authorId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Page<Author>> GetAuthors(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAuthor(Author author, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

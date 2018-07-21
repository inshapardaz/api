using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public AuthorRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Author> AddAuthor(Author author, CancellationToken cancellationToken)
        {
            var item = author.Map<Author, Entities.Library.Author>();
            _databaseContext.Author.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return item.Map<Entities.Library.Author, Author>();
        }

        public async Task UpdateAuthor(Author author, CancellationToken cancellationToken)
        {
            var existingEntity = await _databaseContext.Author
                                                       .SingleOrDefaultAsync(g => g.Id == author.Id,
                                                                             cancellationToken);

            if (existingEntity == null)
            {
                throw new NotFoundException();
            }

            existingEntity.Name = author.Name;
            existingEntity.ImageId = author.ImageId;

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAuthor(int authorId, CancellationToken cancellationToken)
        {
            var author = await _databaseContext.Author.SingleOrDefaultAsync(g => g.Id == authorId, cancellationToken);

            if (author == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.Author.Remove(author);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Page<Author>> GetAuthors(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var author = _databaseContext.Author;

            var count = author.Count();
            var data = await author
                             .Paginate(pageNumber, pageSize)
                             .Select(a => a.Map<Entities.Library.Author, Author>())
                             .ToListAsync(cancellationToken);

            return new Page<Author>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<Author> GetAuthorById(int authorId, CancellationToken cancellationToken)
        {
            var author = await _databaseContext.Author
                                               .SingleOrDefaultAsync(t => t.Id == authorId,
                                                                     cancellationToken);
            return author.Map<Entities.Library.Author, Author>();
        }
    }
}

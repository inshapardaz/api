using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class GenreRepository : IGenreRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public GenreRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Genre> AddGenre(Genre genre, CancellationToken cancellationToken)
        {
            var item = genre.Map<Genre, Entities.Library.Genre>();
            _databaseContext.Genere.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return item.Map<Entities.Library.Genre, Genre>();
        }

        public async Task UpdateGenre(Genre genre, CancellationToken cancellationToken)
        {
            var existingEntity = await _databaseContext.Genere
                                                       .SingleOrDefaultAsync(g => g.Id == genre.Id,
                                                                             cancellationToken);

            if (existingEntity == null)
            {
                throw new NotFoundException();
            }

            existingEntity.Name = genre.Name;

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteGenre(int genereId, CancellationToken cancellationToken)
        {
            var genere = await _databaseContext.Genere.SingleOrDefaultAsync(g => g.Id == genereId, cancellationToken);

            if (genere == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.Genere.Remove(genere);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Genre>> GetGenres(CancellationToken cancellationToken)
        {
            return await _databaseContext.Genere
                                         .Select(t => t.Map<Entities.Library.Genre, Genre>())
                                         .ToListAsync(cancellationToken);
        }

        public async Task<Genre> GetGenreById(int genereId, CancellationToken cancellationToken)
        {
            var genere = await _databaseContext.Genere
                                                    .SingleOrDefaultAsync(t => t.Id == genereId,
                                                                          cancellationToken);
            return genere.Map<Entities.Library.Genre, Genre>();
        }
    }
}
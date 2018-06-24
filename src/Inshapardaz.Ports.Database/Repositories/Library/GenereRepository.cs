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
    public class GenereRepository : IGenereRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public GenereRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Genere> AddGenere(Genere genere, CancellationToken cancellationToken)
        {
            var item = genere.Map<Genere, Entities.Library.Genere>();
            _databaseContext.Genere.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return item.Map<Entities.Library.Genere, Genere>();
        }

        public async Task UpdateGenere(Genere genere, CancellationToken cancellationToken)
        {
            var existingEntity = await _databaseContext.Genere
                                                       .SingleOrDefaultAsync(g => g.Id == genere.Id,
                                                                             cancellationToken);

            if (existingEntity == null)
            {
                throw new NotFoundException();
            }

            existingEntity.Name = genere.Name;

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteGenere(int genereId, CancellationToken cancellationToken)
        {
            var genere = await _databaseContext.Genere.SingleOrDefaultAsync(g => g.Id == genereId, cancellationToken);

            if (genere == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.Genere.Remove(genere);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Genere>> GetGeneres(CancellationToken cancellationToken)
        {
            return await _databaseContext.Genere
                                         .Select(t => t.Map<Entities.Library.Genere, Genere>())
                                         .ToListAsync(cancellationToken);
        }

        public async Task<Genere> GetGenereById(int genereId, CancellationToken cancellationToken)
        {
            var genere = await _databaseContext.Genere
                                                    .SingleOrDefaultAsync(t => t.Id == genereId,
                                                                          cancellationToken);
            return genere.Map<Entities.Library.Genere, Genere>();
        }
    }
}
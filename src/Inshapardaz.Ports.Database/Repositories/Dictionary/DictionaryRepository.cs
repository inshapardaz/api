using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories.Dictionary
{
    public class DictionaryRepository : IDictionaryRepository
    {
        private readonly IDatabaseContext _database;

        public DictionaryRepository(IDatabaseContext database)
        {
            _database = database;
        }

        public async Task<Domain.Entities.Dictionary.Dictionary> AddDictionary(Domain.Entities.Dictionary.Dictionary dictionary, CancellationToken cancellationToken)
        {
            var entity = dictionary.Map();
            await _database.Dictionary.AddAsync(entity, cancellationToken);
            await _database.SaveChangesAsync(cancellationToken);

            return entity.Map();
        }

        public async Task UpdateDictionary(int dictionaryId, Domain.Entities.Dictionary.Dictionary dictionary, CancellationToken cancellationToken)
        {
            var existingDictionary = await _database.Dictionary.SingleOrDefaultAsync(d => d.Id == dictionary.Id, cancellationToken);

            if (existingDictionary == null)
            {
                throw new NotFoundException();
            }

            existingDictionary.Name = dictionary.Name;
            existingDictionary.Language = dictionary.Language;
            existingDictionary.IsPublic = dictionary.IsPublic;

            await _database.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteDictionary(int dictionaryId, CancellationToken cancellationToken)
        {
            var d = await _database.Dictionary.SingleOrDefaultAsync(x => x.Id == dictionaryId, cancellationToken);
            _database.Dictionary.Remove(d);

            await _database.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Domain.Entities.Dictionary.Dictionary>> GetAllDictionaries(CancellationToken cancellationToken)
        {
            return await _database.Dictionary
                                  .Include(d => d.Downloads)
                                  .Select(d => d.Map())
                                  .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Domain.Entities.Dictionary.Dictionary>> GetPublicDictionaries(CancellationToken cancellationToken)
        {
            return await _database.Dictionary
                                  .Include(d => d.Downloads)
                                  .Where(d => d.IsPublic)
                                  .Select(d => d.Map())
                                  .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Domain.Entities.Dictionary.Dictionary>> GetAllDictionariesForUser(Guid userId, CancellationToken cancellationToken)
        {
            return await _database.Dictionary
                                  .Include(d => d.Downloads)
                                  .Where(d =>  d.UserId == userId || d.IsPublic)
                                  .Select(d => d.Map())
                                  .ToListAsync(cancellationToken);
        }

        public async Task<Domain.Entities.Dictionary.Dictionary> GetDictionaryById(int dictionaryId, CancellationToken cancellationToken)
        {
            return (await _database.Dictionary
                                   .Include(d => d.Downloads)
                                   .SingleOrDefaultAsync(d => d.Id == dictionaryId, cancellationToken)).Map();
        }

        public async Task<IEnumerable<Domain.Entities.Dictionary.DictionaryDownload>> GetDictionaryDownloads(int dictionaryId, CancellationToken cancellationToken)
        {
            return await _database.DictionaryDownload
                                   .Where(d => d.DictionaryId == dictionaryId)
                                   .Select(d => d.Map())
                                   .ToListAsync(cancellationToken);
        }

        public async Task<Domain.Entities.Dictionary.DictionaryDownload> GetDictionaryDownloadById(int dictionaryId, string mimeType, CancellationToken cancellationToken)
        {
            return (await _database.DictionaryDownload
                                  .Include(d => d.File)
                                  .Where(d => d.DictionaryId == dictionaryId && d.MimeType == mimeType)
                                  .SingleOrDefaultAsync(cancellationToken))
                                  .Map();
        }

        public Task<Domain.Entities.Dictionary.DictionaryDownload> AddDictionaryDownload(int dictionaryId, string mimeType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

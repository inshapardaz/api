using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories.Dictionary
{
    public class MeaningRepository : IMeaningRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public MeaningRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Meaning> AddMeaning(int dictionaryId, long wordId, Meaning meaning, CancellationToken cancellationToken)
        {
            var word = await _databaseContext.Word.SingleOrDefaultAsync(
                w => w.Id == wordId && w.DictionaryId == dictionaryId,
                cancellationToken);

            if (word == null)
            {
                throw new NotFoundException();
            }

            var item = meaning.Map<Meaning, Entities.Dictionary.Meaning>();
            word.Meaning.Add(item);
            await _databaseContext.SaveChangesAsync(cancellationToken);
            return item.Map<Entities.Dictionary.Meaning, Meaning>();
        }

        public async Task DeleteMeaning(int dictionaryId, long wordId, long meaningId, CancellationToken cancellationToken)
        {
            var meaning = await _databaseContext.Meaning.SingleOrDefaultAsync(
                m => m.Id == meaningId && m.Word.DictionaryId == dictionaryId,
                cancellationToken);

            if (meaning == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.Meaning.Remove(meaning);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Meaning> GetMeaningById(int dictionaryId, long wordId, long meaningId, CancellationToken cancellationToken)
        {
            var meaning = await _databaseContext.Meaning
                                                .SingleOrDefaultAsync(m => m.Id == meaningId && 
                                                                           m.Word.DictionaryId == dictionaryId, 
                                                                      cancellationToken);
            return meaning.Map<Entities.Dictionary.Meaning, Meaning>();
        }

        public async Task UpdateMeaning(int dictionaryId, IFormattable wordId, Meaning meaning, CancellationToken cancellationToken)
        {
            var oldMeaning = await _databaseContext.Meaning.SingleOrDefaultAsync(
                m => m.Id == meaning.Id && m.Word.DictionaryId == dictionaryId,
                cancellationToken);

            if (oldMeaning == null)
            {
                throw new NotFoundException();
            }

            oldMeaning.Context = meaning.Context;
            oldMeaning.Value = meaning.Value;
            oldMeaning.Example = meaning.Example;

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Meaning>> GetMeaningByContext(int dictionaryId, long wordId, string context, CancellationToken cancellationToken)
        {
            return await _databaseContext.Meaning
                                           .Where(m => m.Context == context &&
                                                       m.WordId == wordId &&
                                                       m.Word.DictionaryId == dictionaryId)
                                           .Select(s => s.Map<Entities.Dictionary.Meaning, Meaning>())
                                           .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Meaning>> GetMeaningByWordId(int dictionaryId, long wordId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Meaning
                                          .Where(m => m.WordId == wordId &&
                                                      m.Word.DictionaryId == dictionaryId)
                                          .Select(s => s.Map<Entities.Dictionary.Meaning, Meaning>())
                                          .ToListAsync(cancellationToken);
        }
    }
}

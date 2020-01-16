using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionaries;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories.Dictionaries
{
    public class TranslationRepository : ITranslationRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public TranslationRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Translation> AddTranslation(int dictionaryId, long wordId, Translation translation, CancellationToken cancellationToken)
        {
            var word = await _databaseContext.Word
                                             .SingleOrDefaultAsync(t => t.DictionaryId == dictionaryId &&
                                                                        t.Id == wordId,
                                                                   cancellationToken);
            if (word == null)
            {
                throw new NotFoundException();
            }

            var item = translation.Map();
            word.Translation.Add(item);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return item.Map();
        }

        public async Task DeleteTranslation(int dictionaryId, long wordId, long translationId, CancellationToken cancellationToken)
        {
            var translation = await _databaseContext.Translation
                                                    .SingleOrDefaultAsync(t => t.Word.DictionaryId == dictionaryId && 
                                                                               t.WordId == wordId &&
                                                                               t.Id == translationId,
                                                                          cancellationToken);

            if (translation == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.Translation.Remove(translation);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateTranslation(int dictionaryId, long wordId, Translation translation, CancellationToken cancellationToken)
        {
            var oldTranslation = await _databaseContext.Translation
                                                       .SingleOrDefaultAsync(t => t.Word.DictionaryId == dictionaryId && 
                                                                                  t.WordId == wordId &&
                                                                                  t.Id == translation.Id,
                                                                             cancellationToken);

            if (oldTranslation == null)
            {
                throw new NotFoundException();
            }

            oldTranslation.Language = translation.Language;
            oldTranslation.Value = translation.Value;
            oldTranslation.IsTrasnpiling = translation.IsTrasnpiling;

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Translation> GetTranslationById(int dictionaryId, long wordId, long translationId, CancellationToken cancellationToken)
        {
            var translation = await _databaseContext.Translation
                                         .SingleOrDefaultAsync(t => t.Word.DictionaryId == dictionaryId &&
                                                                    t.WordId == wordId &&
                                                                    t.Id == translationId,
                                                               cancellationToken);
            return translation.Map();
        }

        public async Task<IEnumerable<Translation>> GetTranslationsByLanguage(int dictionaryId, long wordId, Languages language, CancellationToken cancellationToken)
        {
            return await _databaseContext.Translation
                                         .Where(t => t.Word.DictionaryId == dictionaryId &&
                                                     t.WordId == wordId &&
                                                     t.Language == language)
                                         .Select(t => t.Map())
                                         .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Translation>> GetTranslationsByWordId(int dictionaryId, long wordId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Translation
                                         .Where(t => t.Word.DictionaryId == dictionaryId &&
                                                     t.WordId == wordId)
                                         .Select(t => t.Map())
                                         .ToListAsync(cancellationToken);
        }
    }
}

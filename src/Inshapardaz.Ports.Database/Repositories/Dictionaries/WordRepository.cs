﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories.Dictionaries
{
    public class WordRepository : IWordRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public WordRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<WordModel> AddWord(int dictionaryId, WordModel word, CancellationToken cancellationToken)
        {
            var dictionary = await _databaseContext.Dictionary
                                                   .SingleOrDefaultAsync(d => d.Id == dictionaryId, cancellationToken);
            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            var item = word.Map();
            dictionary.Word.Add(item);
            await _databaseContext.SaveChangesAsync(cancellationToken);
            return item.Map();
        }

        public async Task DeleteWord(int dictionaryId, long wordId, CancellationToken cancellationToken)
        {
            var word = await _databaseContext.Word.SingleOrDefaultAsync(
                w => w.Id == wordId && w.DictionaryId == dictionaryId,
                cancellationToken);

            if (word == null || word.Id != wordId)
            {
                throw new NotFoundException();
            }

            _databaseContext.Word.Remove(word);

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateWord(int dictionaryId, WordModel word, CancellationToken cancellationToken)
        {
            var oldWord = await _databaseContext.Word.SingleOrDefaultAsync(
                w => w.Id == word.Id & w.DictionaryId == dictionaryId,
                cancellationToken);

            if (oldWord == null || oldWord.Id != word.Id)
            {
                throw new NotFoundException();
            }

            oldWord.Title = word.Title;
            oldWord.TitleWithMovements = word.TitleWithMovements;
            oldWord.Pronunciation = word.Pronunciation;
            oldWord.Description = word.Description;
            oldWord.Attributes = word.Attributes;
            oldWord.Language = word.Language;

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<WordModel> GetWordById(int dictionaryId, long wordId, CancellationToken cancellationToken)
        {
            var word = await _databaseContext.Word.SingleOrDefaultAsync(w => w.Id == wordId && w.DictionaryId == dictionaryId, cancellationToken);
            return word.Map();
        }

        public async Task<WordModel> GetWordByTitle(int dictionaryId, string title, CancellationToken cancellationToken)
        {
            var word = await _databaseContext.Word.SingleOrDefaultAsync(w => w.Title == title && w.DictionaryId == dictionaryId, cancellationToken);
            return word.Map();
        }

        public async Task<Page<WordModel>> GetWordsById(int dictionaryId, IEnumerable<long> wordIds, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var wordIndices = dictionaryId > 0
                ? _databaseContext.Word.Where(
                    x => x.DictionaryId == dictionaryId && wordIds.Contains(x.Id))
                : _databaseContext.Word.Where(x => wordIds.Contains(x.Id));

            var count = wordIndices.Count();
            var data = await wordIndices
                             .Paginate(pageNumber, pageSize)
                             .Select(w => w.Map())
                             .ToListAsync(cancellationToken);

            return new Page<WordModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }


        public async Task<IEnumerable<WordModel>> GetWordsByTitles(int dictionaryId, IEnumerable<string> titles, CancellationToken cancellationToken)
        {
            return await _databaseContext.Word
                                  .Where(x => x.DictionaryId == dictionaryId && titles.Contains(x.Title))
                                  .Select(w => w.Map())
                                  .ToListAsync(cancellationToken);
        }

        public async Task<Page<WordModel>> GetWordsContaining(int dictionaryId, string searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var words = dictionaryId > 0
                ? _databaseContext.Word.Where(
                    x => x.DictionaryId == dictionaryId && x.Title.StartsWith(searchTerm))
                : _databaseContext.Word.Where(x => x.Title.StartsWith(searchTerm));

            var count = words.Count();
            var data = await words
                             .Paginate(pageNumber, pageSize)
                             .Select(w => w.Map())
                             .ToListAsync(cancellationToken);

            return new Page<WordModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<Page<WordModel>> GetWords(int dictionaryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var words = _databaseContext.Word.Where(x => x.DictionaryId == dictionaryId);

            var count = words.Count();
            var data = await words
                             .Paginate(pageNumber, pageSize)
                             .Select(w => w.Map())
                             .ToListAsync(cancellationToken);

            return new Page<WordModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }

        public async Task<int> GetWordCountByDictionary(int dictionaryId, CancellationToken cancellationToken)
        {
            return  await _databaseContext.Word.CountAsync(d => d.DictionaryId == dictionaryId, cancellationToken);
        }

        public async Task<Page<WordModel>> GetWordsStartingWith(int dictionaryId, string startingWith, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var words = _databaseContext.Word.Where(x => x.DictionaryId == dictionaryId);

            var count = words.Count();
            var data = await words
                             .Where(w => w.Title.StartsWith(startingWith))
                             .Paginate(pageNumber, pageSize)
                             .Select(w => w.Map())
                             .ToListAsync(cancellationToken);

            return new Page<WordModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = data
            };
        }
    }
}
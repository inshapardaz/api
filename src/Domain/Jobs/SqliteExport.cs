using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Inshapardaz.Data.Entities;
using Inshapardaz.Domain.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using File = System.IO.File;
using Languages = Inshapardaz.Data.Entities.Languages;
using Meaning = Inshapardaz.Data.Entities.Meaning;
using RelationType = Inshapardaz.Data.Entities.RelationType;
using Translation = Inshapardaz.Data.Entities.Translation;
using Word = Inshapardaz.Data.Entities.Word;
using WordDetail = Inshapardaz.Data.Entities.WordDetail;
using WordRelation = Inshapardaz.Data.Entities.WordRelation;

namespace Inshapardaz.Domain.Jobs
{
    public class SqliteExport
    {

        private readonly ILogger<SqliteExport> _logger;
        private readonly IDatabaseContext _databaseContext;

        public SqliteExport(ILoggerFactory loggerFactory, IDatabaseContext databaseContext)
        {
            _logger = loggerFactory.CreateLogger<SqliteExport>();
            _databaseContext = databaseContext;
        }

        public void ExportDictionary(int dictionaryId)
        {
            var sqlitePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.dat");
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = sqlitePath,
                Mode = SqliteOpenMode.ReadWriteCreate
            };

            var optionsBuilder = new DbContextOptionsBuilder<Data.DictionaryDatabase>();
            optionsBuilder.UseSqlite(connectionString.ConnectionString);


            using (var database = new Data.DictionaryDatabase(optionsBuilder.Options))
            {

                _logger.LogDebug("Starting creating target database");
                database.Database.Migrate();
                _logger.LogDebug("Target database created");

                var wordIdMap = new Dictionary<long, long>();
                var relationShips = new List<(long SourceWord, long RelatedWord, RelationType RelationType)>();

                var dictionaries = _databaseContext.Dictionary.Where(d => d.Id == dictionaryId)
                                                   .Include(d => d.Word)
                                                   .ThenInclude(w => w.WordDetail)
                                                   .ThenInclude(wd => wd.Meaning)
                                                   .Include(d => d.Word)
                                                   .ThenInclude(w => w.WordDetail)
                                                   .ThenInclude(wd => wd.Translation)
                                                   .Include(d => d.Word)
                                                   .ThenInclude(w => w.WordRelationRelatedWord)
                                                   .Include(d => d.Word)
                                                   .ThenInclude(w => w.WordRelationSourceWord);

                _logger.LogDebug("Starting migrating data");

                foreach (var dictionary in dictionaries)
                {
                    int count = 0;
                    foreach (var word in dictionary.Word)
                    {
                        var newWord = MapWord(word);
                        database.Word.Add(newWord);
                        database.SaveChanges();
                        wordIdMap.Add(word.Id, newWord.Id);

                        foreach (var relation in word.WordRelationRelatedWord)
                        {
                            var newRelation =
                                (
                                SourceWord : relation.SourceWordId,
                                RelatedWord : relation.RelatedWordId,
                                RelationType : (RelationType) relation.RelationType
                                );
                            relationShips.Add(newRelation);
                        }

                        database.SaveChanges();
                        count++;
                        if (count > 1000) break;
                    }
                }

                _logger.LogDebug("Creating relations");

                int count2 = 0;

                foreach (var relation in relationShips)
                {
                    if (wordIdMap.ContainsKey(relation.SourceWord) &&
                        wordIdMap.ContainsKey(relation.RelatedWord))
                    {
                        var sourceWordId = wordIdMap[relation.SourceWord];
                        var relatedWordId = wordIdMap[relation.RelatedWord];
                        database.WordRelation.Add(new WordRelation
                        {
                            SourceWordId = sourceWordId,
                            RelatedWordId = relatedWordId,
                            RelationType = relation.RelationType
                        });
                        database.SaveChanges();
                    }
                    count2++;
                    if (count2 > 1000) break;
                }

                _logger.LogDebug("Data migration completed");
            }

            var bytes = File.ReadAllBytes(sqlitePath);
            var file = new Model.File
            {
                MimeType = MimeTypes.SqlLite,
                Contents = bytes,
                FileName = $"{dictionaryId}.dat",
                DateCreated = DateTime.UtcNow,
                LiveUntil = DateTime.MaxValue
            };


            var exisitngDownload = _databaseContext.DictionaryDownload
                                                   .SingleOrDefault(d => d.DictionaryId == dictionaryId && d.MimeType == MimeTypes.SqlLite);
            if (exisitngDownload != null)
            {
                _databaseContext.File.Remove(exisitngDownload.File);
                _databaseContext.DictionaryDownload.Remove(exisitngDownload);
            }

            _databaseContext.File.Add(file);

            _databaseContext.DictionaryDownload.Add(new DictionaryDownload
            {
                DictionaryId = dictionaryId,
                MimeType = MimeTypes.SqlLite,
                File = file
            });
            _databaseContext.SaveChanges();
        }

        private Word MapWord(Model.Word word)
        {
            return new Word
            {
                Title = word.Title,
                TitleWithMovements = word.TitleWithMovements,
                Pronunciation = word.Pronunciation,
                Description = word.Description,
                WordDetail = MapDetails(word.WordDetail)
            };
        }
        
        private ICollection<WordDetail> MapDetails(IEnumerable<Model.WordDetail> details)
        {
            var result = new List<WordDetail>();
            foreach (var detail in details)
            {
                 result.Add(MapDetail(detail));
            }

            return result;
        }

        private WordDetail MapDetail(Model.WordDetail detail)
        {
            return new WordDetail
            {
                Attributes = (GrammaticalTypes) detail.Attributes,
                Language = (Languages) detail.Language,
                Translation = MapTranslations(detail.Translation),
                Meaning = MapMeanings(detail.Meaning)
            };
        }

        private ICollection<Meaning> MapMeanings(IEnumerable<Model.Meaning> meanings)
        {
            var result = new List<Meaning>();
            foreach (var meaning in meanings)
            {
                result.Add(MapMeaning(meaning));
            }

            return result;
        }

        private Meaning MapMeaning(Model.Meaning meaning)
        {
            return new Meaning
            {
                Value = meaning.Value,
                Context = meaning.Context,
                Example =  meaning.Example,
            };
        }

        private ICollection<Translation> MapTranslations(IEnumerable<Model.Translation> translations)
        {
            var result = new List<Translation>();
            foreach (var translation in translations)
            {
                result.Add(MapTranslation(translation));
            }

            return result;
        }

        private Translation MapTranslation(Model.Translation translation)
        {
            return new Translation
            {
                Language = (Languages)translation.Language,
                Value = translation.Value
            };
        }
    }
}

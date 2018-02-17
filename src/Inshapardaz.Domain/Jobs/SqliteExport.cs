using System.Collections.Generic;
using Inshapardaz.Data.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Languages = Inshapardaz.Data.Entities.Languages;
using Meaning = Inshapardaz.Data.Entities.Meaning;
using Translation = Inshapardaz.Data.Entities.Translation;
using Word = Inshapardaz.Data.Entities.Word;

namespace Inshapardaz.Domain.Jobs
{
    public class SqliteExport
    {
        private readonly TelemetryClient _telemetryClient = new TelemetryClient(TelemetryConfiguration.Active);

        private readonly ILogger<SqliteExport> _logger;

        public SqliteExport(ILogger<SqliteExport> logger)
        {
            _logger = logger;
        }

        public void ExportDictionary(int dictionaryId)
        {
            //_logger.LogDebug($"Started export for dictionary {dictionaryId}");

            //var operation = _telemetryClient.StartOperation<RequestTelemetry>($"Export dictionary {dictionaryId}");

            //try
            //{
            //    var sqlitePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.dat");
            //    var connectionString = new SqliteConnectionStringBuilder
            //    {
            //        DataSource = sqlitePath,
            //        Mode = SqliteOpenMode.ReadWriteCreate
            //    };

            //    var optionsBuilder = new DbContextOptionsBuilder<Data.DictionaryDatabase>();
            //    optionsBuilder.UseSqlite(connectionString.ConnectionString)
            //                  .EnableSensitiveDataLogging();


            //    using (var targetDatabase = new Data.DictionaryDatabase(optionsBuilder.Options))
            //    {

            //        _logger.LogDebug($"Starting creating target database for dictionary {dictionaryId}");
            //        targetDatabase.Database.EnsureCreated();
            //        _logger.LogDebug($"Target database created for dictionary {dictionaryId}");

            //        var wordIdMap = new Dictionary<long, long>();
            //        var relationShips = new List<(long SourceWord, long RelatedWord, RelationType RelationType)>();

            //        var sourceDictionary = _databaseContext.Dictionary.FirstOrDefault(d => d.Id == dictionaryId);
            //        ;

            //        _logger.LogDebug($"Starting migrating data for dictionary {dictionaryId}");

            //        targetDatabase.Dictionary.Add(new Data.Entities.Dictionary
            //        {
            //            Id = sourceDictionary.Id,
            //            Name = sourceDictionary.Name,
            //            Language = (Languages) sourceDictionary.Language,
            //            IsPublic = sourceDictionary.IsPublic
            //        });

            //        targetDatabase.SaveChanges();
            //        _telemetryClient.TrackTrace($"done dictionary creation");

            //        var newDictionary = targetDatabase.Dictionary.First();
            //        var wordCount = _databaseContext.Word.Count(d => d.DictionaryId == dictionaryId);
            //        var pageSize = 1000;

            //        var numberOfPages = (wordCount / pageSize) + 1;

            //        int count = 0;

            //        for (int i = 1; i <= numberOfPages; i++)
            //        {
            //            _logger.LogDebug($"Processing page {i} or {numberOfPages} for dictionary {dictionaryId}");
            //            var words = _databaseContext.Word
            //                                        .Where(w => w.DictionaryId == dictionaryId)
            //                                        .OrderBy(w => w.Title)
            //                                        .Paginate(i, pageSize)
            //                                        .Include(wd => wd.Meaning)
            //                                        .Include(wd => wd.Translation)
            //                                        .Include(w => w.Relations)
            //                                        .Include(w => w.WordRelationSourceWord);

            //            foreach (var word in words)
            //            {
            //                var newWord = MapWord(word);
            //                newWord.DictionaryId = newDictionary.Id;
            //                targetDatabase.Word.Add(newWord);
            //                targetDatabase.SaveChanges();
            //                wordIdMap.Add(word.Id, newWord.Id);

            //                foreach (var relation in word.Relations)
            //                {
            //                    var newRelation =
            //                        (
            //                        SourceWord: relation.SourceWordId,
            //                        RelatedWord: relation.RelatedWordId,
            //                        RelationType: (RelationType) relation.RelationType
            //                        );
            //                    relationShips.Add(newRelation);
            //                }

            //                targetDatabase.SaveChanges();
            //                count++;
            //                _telemetryClient.TrackTrace($"Adding words {count/wordCount*100}%");
            //            }
            //        }

            //        _logger.LogDebug($"Creating relationships for dictionary {dictionaryId}");

            //        int count2 = 0;

            //        foreach (var relation in relationShips)
            //        {
            //            if (wordIdMap.ContainsKey(relation.SourceWord) &&
            //                wordIdMap.ContainsKey(relation.RelatedWord))
            //            {
            //                var sourceWordId = wordIdMap[relation.SourceWord];
            //                var relatedWordId = wordIdMap[relation.RelatedWord];
            //                targetDatabase.WordRelation.Add(new WordRelation
            //                {
            //                    SourceWordId = sourceWordId,
            //                    RelatedWordId = relatedWordId,
            //                    RelationType = relation.RelationType
            //                });
            //                targetDatabase.SaveChanges();
            //            }
            //            count2++;
            //            _telemetryClient.TrackTrace($"Adding relationships {count2 / relationShips.Count * 100}%");
            //        }

            //    }

            //    _logger.LogDebug($"Writing sqlite file to {sqlitePath} for dictionary {dictionaryId}");

            //    var bytes = File.ReadAllBytes(sqlitePath);
            //    var file = new Database.Entities.File
            //    {
            //        MimeType = MimeTypes.SqlLite,
            //        Contents = bytes,
            //        FileName = $"{dictionaryId}.dat",
            //        DateCreated = DateTime.UtcNow,
            //        LiveUntil = DateTime.MaxValue
            //    };


            //    var exisitngDownload = _databaseContext.DictionaryDownload
            //                                           .Include(d => d.File)
            //                                           .SingleOrDefault(d => d.DictionaryId == dictionaryId && d.MimeType == MimeTypes.SqlLite);
            //    if (exisitngDownload != null)
            //    {
            //        _databaseContext.DictionaryDownload.Remove(exisitngDownload);

            //        if (exisitngDownload.File != null)
            //        {
            //            _databaseContext.File.Remove(exisitngDownload.File);
            //        }
            //    }

            //    _databaseContext.File.Add(file);

            //    _databaseContext.DictionaryDownload.Add(new DictionaryDownload
            //    {
            //        DictionaryId = dictionaryId,
            //        MimeType = MimeTypes.SqlLite,
            //        File = file
            //    });

            //    _databaseContext.SaveChanges();
            //    _logger.LogDebug($"Finished export for dictionary {dictionaryId}");
            //}
            //catch (System.Exception ex)
            //{
            //    _logger.LogError(ex, $"Finished export for dictionary with errors {dictionaryId}");
            //    _telemetryClient.TrackException(ex);
            //}
            //finally
            //{
            //    _telemetryClient.StopOperation(operation);
            //}

        }

        private Word MapWord(Entities.Word word)
        {
            return new Word
            {
                Title = word.Title,
                TitleWithMovements = word.TitleWithMovements,
                Pronunciation = word.Pronunciation,
                Description = word.Description,
                Attributes = (GrammaticalTypes)word.Attributes,
                Language = (Languages)word.Language,
                Meaning = MapMeanings(word.Meaning),
                Translation = MapTranslations(word.Translation)
            };
        }
        
        private ICollection<Meaning> MapMeanings(IEnumerable<Entities.Meaning> meanings)
        {
            var result = new List<Meaning>();
            foreach (var meaning in meanings)
            {
                result.Add(MapMeaning(meaning));
            }

            return result;
        }

        private Meaning MapMeaning(Entities.Meaning meaning)
        {
            return new Meaning
            {
                Value = meaning.Value,
                Context = meaning.Context,
                Example =  meaning.Example,
            };
        }

        private ICollection<Translation> MapTranslations(IEnumerable<Entities.Translation> translations)
        {
            var result = new List<Translation>();
            foreach (var translation in translations)
            {
                result.Add(MapTranslation(translation));
            }

            return result;
        }

        private Translation MapTranslation(Entities.Translation translation)
        {
            return new Translation
            {
                Language = (Languages)translation.Language,
                Value = translation.Value
            };
        }
    }
}

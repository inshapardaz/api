using System;
using System.Collections.Generic;
using Inshapardaz.Domain.Database.Entities;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Domain.IndexingService
{
    public interface IWriteDictionaryIndex
    {
        void AddWord(int dictionaryId, Word word);

        void CreateIndex(int dictionaryId, IEnumerable<Word> words);
    }

    public class DictionaryIndexWriter : IWriteDictionaryIndex
    {
        private readonly IProvideIndexLocation _indexLocationProvider;
        private readonly ILogger<DictionaryIndexWriter> _logger;

        public DictionaryIndexWriter(IProvideIndexLocation indexLocationProvider, ILogger<DictionaryIndexWriter> logger)
        {
            _indexLocationProvider = indexLocationProvider;
            _logger = logger;
        }

        public void AddWord(int dictionaryId, Word word)
        {
            var directory = FSDirectory.Open(_indexLocationProvider.GetDictionaryIndexFolder(dictionaryId));
            _logger.LogDebug("Writing index for dictionary {0} to path : {1}", dictionaryId, directory);

            Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            using (IndexWriter indexWriter = new IndexWriter(directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer)))
            {
                var doc = new Document
                {
                    new Int64Field("id", word.Id, Field.Store.YES),
                    new TextField("title", word.Title, Field.Store.YES)
                };

                _logger.LogDebug("Adding object with id : {0} and title : '{1}'", word.Id, word.Title);
                indexWriter.AddDocument(doc);
            }

            _logger.LogDebug("Finished writing index for dictionary {0} to path : {1}", dictionaryId, directory);
        }

        public void CreateIndex(int dictionaryId, IEnumerable<Word> words)
        {
            var indexFolder = _indexLocationProvider.GetDictionaryIndexFolder(dictionaryId);
            var directory = FSDirectory.Open(indexFolder);

            bool indexExists = DirectoryReader.IndexExists(directory);
            if (indexExists)
            {
                _logger.LogWarning("Index for dictionary {0} already exists at path : {1}. Removing now.", dictionaryId, directory);
                System.IO.Directory.Delete(indexFolder, true);
            }

            _logger.LogDebug("Writing index for dictionary {0} to path : {1}", dictionaryId, directory);
            
            Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            using (IndexWriter indexWriter = new IndexWriter(directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer)))
            {
                foreach (var word in words)
                {
                    var doc = new Document
                    {
                        new Int64Field("id", word.Id, Field.Store.YES),
                        new TextField("title", word.Title, Field.Store.YES)
                    };

                    indexWriter.AddDocument(doc);
                }
            }

            _logger.LogDebug("Finished writing index for dictionary {0} to path : {1}", dictionaryId, directory);
        }
    }
}

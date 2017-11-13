using System.Collections.Generic;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    [TestFixture]
    public class GetDownloadByDictionaryIdQueryHandlerTests : DatabaseTest
    {
        private readonly GetDownloadByDictionaryIdQueryHandler _handler;
        private readonly Dictionary _dictionary;
        private readonly File _file;

        public GetDownloadByDictionaryIdQueryHandlerTests()
        {
            _file = Builder<File>
                .CreateNew()
                .With(f => f.MimeType = MimeTypes.SqlLite)
                .Build();
            var download = Builder<DictionaryDownload>
                .CreateNew()
                .With(d => d.File = _file)
                .With(d => d.MimeType = MimeTypes.SqlLite)
                .Build();
            _dictionary = Builder<Dictionary>
                .CreateNew()
                .With(d => d.Id = 23)
                .With(d => d.Downloads = new List<DictionaryDownload>{download})
                .Build();

            DbContext.Dictionary.Add(_dictionary);
            DbContext.SaveChanges();

            _handler = new GetDownloadByDictionaryIdQueryHandler(DbContext);
        }

        [Test]
        public async Task WhenDownloadRequested_shouldReturnDownloadForCorretDictionary()
        {
            var result = await _handler.ExecuteAsync(new GetDownloadByDictionaryIdQuery
            {
                DictionaryId = _dictionary.Id,
                MimeType = MimeTypes.SqlLite
            });

            result.ShouldNotBeNull();
            result.ShouldBe(_file);
        }

        [Test]
        public async Task WhenDownloadForIncorrectMimeType_ShouldReturnNull()
        {
            var result = await _handler.ExecuteAsync(new GetDownloadByDictionaryIdQuery
            {
                DictionaryId = _dictionary.Id,
                MimeType = "application/binary"
            });

            result.ShouldBeNull();
        }

        [Test]
        public async Task WhenDownloadForDictionaryThatDoesnotExists_ShouldReturnNull()
        {
            var result = await _handler.ExecuteAsync(new GetDownloadByDictionaryIdQuery { DictionaryId = -9 });

            result.ShouldBeNull();
        }
    }
}
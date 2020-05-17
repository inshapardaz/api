using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Bogus;
using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Ports.Database;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class ChapterDataBuilder
    {
        private readonly List<ChapterDto> _chapters = new List<ChapterDto>();
        private readonly List<FileDto> _files = new List<FileDto>();
        private readonly IDbConnection _connection;
        private readonly BooksDataBuilder _booksBuilder;
        private int _contentCount;
        private int _libraryId;

        public ChapterDataBuilder(IProvideConnection connectionProvider,
                                    BooksDataBuilder booksBuilder)
        {
            _connection = connectionProvider.GetConnection();
            _booksBuilder = booksBuilder;
        }

        public ChapterDataBuilder WithContents(int count = 1)
        {
            _contentCount = count;
            return this;
        }

        internal ChapterDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public ChapterDto Build() => Build(1).Single();

        public IEnumerable<ChapterDto> Build(int count)
        {
            var fixture = new Fixture();
            var book = _booksBuilder.WithLibrary(_libraryId).Build();

            for (int i = 0; i < count; i++)
            {
                var chapter = fixture.Build<ChapterDto>()
                                     .With(c => c.BookId, book.Id)
                                     .Create();

                _connection.AddChapter(chapter);
                _chapters.Add(chapter);

                for (int j = 0; j < _contentCount; j++)
                {
                    var chapterContent = fixture.Build<FileDto>()
                                                 .With(a => a.FilePath, Random.BlobUrl)
                                                 .With(c => c.MimeType, MimeTypes.Pdf)
                                                 .With(a => a.IsPublic, true)
                                                 .Create();
                    _connection.AddFile(chapterContent);
                    _files.Add(chapterContent);

                    var content = fixture.Build<ChapterContentDto>()
                        .With(c => c.ChapterId, chapter.Id)
                        .With(c => c.FileId, chapterContent.Id)
                        .With(c => c.Language, Random.Locale)
                        .Create();

                    _connection.AddChapterContent(content);
                }
            }

            return _chapters;
        }
    }
}

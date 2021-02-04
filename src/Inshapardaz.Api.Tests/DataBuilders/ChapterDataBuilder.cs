using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Api.Tests.Helpers;

namespace Inshapardaz.Api.Tests.DataBuilders
{
    public class ChapterDataBuilder
    {
        private readonly List<ChapterDto> _chapters = new List<ChapterDto>();
        private readonly List<ChapterContentDto> _contents = new List<ChapterContentDto>();
        private readonly IDbConnection _connection;
        private readonly BooksDataBuilder _booksBuilder;
        private int _contentCount;
        private int _libraryId;
        private bool _public;
        private string _language;

        public IEnumerable<ChapterContentDto> Contents => _contents;
        public IEnumerable<ChapterDto> Chapters => _chapters;

        public ChapterDataBuilder(IProvideConnection connectionProvider,
                                    BooksDataBuilder booksBuilder)
        {
            _connection = connectionProvider.GetConnection();
            _booksBuilder = booksBuilder;
        }

        internal ChapterDataBuilder Public()
        {
            _public = true;
            return this;
        }

        internal ChapterDataBuilder Private()
        {
            _public = false;
            return this;
        }

        public ChapterDataBuilder WithContents(int count = 1)
        {
            _contentCount = count;
            return this;
        }

        public ChapterDataBuilder WithoutContents()
        {
            _contentCount = 0;
            return this;
        }

        internal ChapterDataBuilder WithContentLanguage(string language)
        {
            _language = language;
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
            var book = _booksBuilder.WithLibrary(_libraryId).IsPublic(_public).Build();

            for (int i = 0; i < count; i++)
            {
                var chapter = fixture.Build<ChapterDto>()
                                     .With(c => c.BookId, book.Id)
                                     .Create();

                _connection.AddChapter(chapter);
                _chapters.Add(chapter);

                for (int j = 0; j < _contentCount; j++)
                {
                    var content = fixture.Build<ChapterContentDto>()
                        .With(c => c.ChapterId, chapter.Id)
                        .With(c => c.Text, Random.String)
                        .With(c => c.Language, _language ?? Helpers.Random.Locale)
                        .Create();

                    _contents.Add(content);

                    _connection.AddChapterContent(content);
                }
            }

            return _chapters;
        }

        public void CleanUp()
        {
            _connection.DeleteChapterContents(_contents);
            _connection.DeleteChapters(_chapters);
        }
    }
}

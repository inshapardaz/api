using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Ports.Database;
using NUnit.Framework;
using SQLitePCL;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class ChapterDataBuilder
    {
        private readonly List<ChapterDto> _chapters = new List<ChapterDto>();
        private readonly List<FileDto> _files = new List<FileDto>();
        private readonly List<ChapterContentDto> _contents = new List<ChapterContentDto>();
        private readonly FakeFileStorage _fileStorage;
        private readonly IDbConnection _connection;
        private readonly BooksDataBuilder _booksBuilder;
        private int _contentCount;
        private int _libraryId;
        private bool _public;
        private string _mimeType = MimeTypes.Pdf;

        public IEnumerable<ChapterContentDto> Contents => _contents;
        public IEnumerable<ChapterDto> Chapters => _chapters;

        public IEnumerable<FileDto> Files => _files;

        public ChapterDataBuilder(IProvideConnection connectionProvider,
                                    BooksDataBuilder booksBuilder,
                                    IFileStorage fileStorage)
        {
            _connection = connectionProvider.GetConnection();
            _booksBuilder = booksBuilder;
            _fileStorage = fileStorage as FakeFileStorage;
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

        public ChapterDataBuilder WithMimeType(string mimeType)
        {
            _mimeType = mimeType;
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
                    var file = fixture.Build<FileDto>()
                                                    .With(a => a.FilePath, Helpers.Random.BlobUrl)
                                                    .With(c => c.MimeType, _mimeType)
                                                    .With(a => a.IsPublic, true)
                                                    .Create();
                    _fileStorage.SetupFileContents(file.FilePath, Helpers.Random.Bytes);
                    _connection.AddFile(file);
                    _files.Add(file);

                    var content = fixture.Build<ChapterContentDto>()
                        .With(c => c.ChapterId, chapter.Id)
                        .With(c => c.FileId, file.Id)
                        .With(c => c.Language, Helpers.Random.Name)
                        .Create();

                    _contents.Add(content);

                    _connection.AddChapterContent(content);
                }
            }

            return _chapters;
        }
    }
}

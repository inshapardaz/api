using System.Collections.Generic;
using System.Linq;
using Bogus;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Ports.Database;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class ChapterDataBuilder
    {
        private readonly List<ChapterDto> _chapters = new List<ChapterDto>();
        private readonly List<FileDto> _files = new List<FileDto>();
        private readonly IProvideConnection _connectionProvider;
        private bool _hasContent = false;
        private bool? _isPublic = false;
        private string _contentLink = string.Empty;

        public ChapterDataBuilder(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public ChapterDataBuilder WithContentLink(string contentLink)
        {
            _contentLink = contentLink;
            return this;
        }

        public ChapterDataBuilder WithContents()
        {
            _hasContent = true;
            return this;
        }

        public ChapterDataBuilder AsPrivate()
        {
            _isPublic = false;
            return this;
        }

        public ChapterDataBuilder AsPublic()
        {
            _isPublic = true;
            return this;
        }

        public ChapterDataBuilder WithChapters(string a, int count, bool? isPublic = null, bool hasContent = false)
        {
            return this;
        }

        public ChapterDto Build() => Build(1).Single();

        public IEnumerable<ChapterDto> Build(int count)
        {
            //var author = new Faker<AuthorDto>()
            //             .RuleFor(c => c.Id, 0)
            //             .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10))
            //             .RuleFor(c => c.ImageId, f => f.Random.Int(1))
            //             .Generate();

            //var book = new Faker<BookDto>()
            //           .RuleFor(c => c.Id, 0)
            //           .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
            //           .RuleFor(c => c.AuthorId, author.Id)
            //           .RuleFor(c => c.IsPublic, f => _isPublic ?? f.Random.Bool())
            //           .Generate();

            //var chapterIndex = 1;
            //var chapters = new Faker<ChapterDto>()
            //               .RuleFor(c => c.Id, 0)
            //               .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
            //               .RuleFor(c => c.ChapterNumber, chapterIndex++)
            //               .Generate(count);

            //var chapterContent = new Faker<ChapterContent>()
            //                     .RuleFor(c => c.MimeType, "text/markdown")
            //                     .RuleFor(c => c.ContentUrl, f => _contentLink ?? f.Internet.Url());

            //if (_hasContent)
            //{
            //    foreach (var chapter in chapters)
            //    {
            //        chapter.Contents = new List<ChapterContent>()
            //        {
            //            chapterContent.RuleFor(c => c.Chapter, chapter).Generate()
            //        };
            //    }
            //}

            //foreach (var chapter in chapters)
            //{
            //    chapter.Book = book;
            //}

            //_chapters.AddRange(chapters);
            //_context.Chapter.AddRange(_chapters);

            //_context.SaveChanges();

            //return _chapters;
            return null;
        }
    }
}

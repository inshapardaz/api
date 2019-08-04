using System.Collections.Generic;
using System.Linq;
using Bogus;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Entities;
using Inshapardaz.Ports.Database.Entities.Library;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class ChapterDataBuilder
    {

        private readonly IDatabaseContext _context;
        private readonly List<Chapter> _chapters = new List<Chapter>();
        private readonly List<File> _files = new List<File>();

        public ChapterDataBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public ChapterDataBuilder WithChapters(int count, bool? isPublic = null, bool hasContent = false)
        {
            var author = new Faker<Author>()
                         .RuleFor(c => c.Id, 0)
                         .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10))
                         .RuleFor(c => c.ImageId, f => f.Random.Int(1))
                         .Generate();

            var book = new Faker<Book>()
                        .RuleFor(c => c.Id, 0)
                        .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
                        .RuleFor(c => c.Author, author)
                        .RuleFor(c => c.IsPublic, f => isPublic ?? f.Random.Bool())
                        .Generate();

            var chapterIndex = 0;
            var chapters = new Faker<Chapter>()
                           .RuleFor(c => c.Id, 0)
                           .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
                           .RuleFor(c => c.ChapterNumber, chapterIndex++)
                           .Generate(count);

            var chapterContent = new Faker<ChapterContent>()
                .RuleFor(c => c.MimeType, "text/markdown")
                .RuleFor(c => c.ContentUrl, f => f.Internet.Url());

            if (hasContent)
            {
                foreach (var chapter in chapters)
                {
                    chapter.Contents = new List<ChapterContent>()
                    {
                        chapterContent.RuleFor(c => c.Chapter, chapter).Generate()
                    };
                }
            }

            foreach (var chapter in chapters)
            {
                chapter.Book = book;
            }

            _chapters.AddRange(chapters);
            return this;
        }
        
        public IEnumerable<Chapter> Build()
        {
            _context.Chapter.AddRange(_chapters);
            //_context.File.AddRange(_files);
            _context.SaveChanges();

            return _chapters;
        }

        public Chapter GetById(int id)
        {
            return _context.Chapter.SingleOrDefault(x => x.Id == id);
        }
    }
}

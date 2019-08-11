using System.Collections.Generic;
using System.Linq;
using Bogus;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Entities.Library;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class SeriesDataBuilder
    {

        private readonly IDatabaseContext _context;
        private int _bookCount;
        
        public SeriesDataBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public SeriesDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }

        public Series Build() => Build(1).Single();

        public IEnumerable<Series> Build(int count)
        {
            var authorGenerator = new Faker<Author>()
                                  .RuleFor(c => c.Id, 0)
                                  .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10));
            
            var series = new Faker<Series>()
                         .RuleFor(c => c.Id, 0)
                         .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10))
                         .RuleFor(c => c.Description, f => f.Random.Words(30))
                         .RuleFor(c => c.ImageId, f => f.Random.Int())
                         .Generate(count);

            

            foreach (var s in series)
            {
                new Faker<Book>()
                    .RuleFor(c => c.Id, 0)
                    .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
                    .RuleFor(c => c.Author, f => authorGenerator.Generate())
                    .Generate(_bookCount)
                    .ForEach(b => s.Books.Add(b));
            }
            
            _context.Series.AddRange(series);
            
            _context.SaveChanges();

            return series;
        }

        public Series GetById(int id)
        {
            return _context.Series.SingleOrDefault(x => x.Id == id);
        }
    }
}

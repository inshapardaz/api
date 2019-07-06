using System.Collections.Generic;
using System.Linq;
using Bogus;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Entities;
using Inshapardaz.Ports.Database.Entities.Library;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class SeriesDataBuilder
    {

        private readonly IDatabaseContext _context;
        private readonly List<Series> _series = new List<Series>();

        public SeriesDataBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public SeriesDataBuilder WithSeries(int count, int bookCount = 0)
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
                    .Generate(bookCount)
                    .ForEach(b => s.Books.Add(b));
            }
            
            _series.AddRange(series);
            return this;
        }
        
        public IEnumerable<Series> Build()
        {
            _context.Series.AddRange(_series);
            
            _context.SaveChanges();

            return _series;
        }

        public Series GetById(int id)
        {
            return _context.Series.SingleOrDefault(x => x.Id == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.UnitTests.Fakes
{
    public class MockDatabase : IDatabase
    {
        public DbSet<Dictionary> Dictionaries { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<Meaning> Meanings { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<WordDetail> WordDetails { get; set; }
        public DbSet<WordRelation> WordRelations { get; set; }
        public int SaveChanges()
        {
            WasSavedCalled = true;
            return 1;
        }

        public bool WasSavedCalled { get; private set; }
    }
}

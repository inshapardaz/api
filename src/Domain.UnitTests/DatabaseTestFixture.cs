using System;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.UnitTests
{
    public abstract class DatabaseTestFixture
    {
        protected DatabaseContext _database;

        public DatabaseTestFixture()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _database.Database.EnsureCreated();
        }
        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }
    }
}

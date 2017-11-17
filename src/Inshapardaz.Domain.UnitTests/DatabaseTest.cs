using System;
using Inshapardaz.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.UnitTests
{
    public abstract class DatabaseTest: IDisposable 
    {
        protected readonly DatabaseContext DbContext;
        
        protected DatabaseTest()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            DbContext = new DatabaseContext(inMemoryDataContextOptions);
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext?.Dispose();
        }
    }
}

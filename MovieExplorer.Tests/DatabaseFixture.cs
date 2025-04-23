using Microsoft.EntityFrameworkCore;
using MovieExplorer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieExplorer.Tests
{
    public class DatabaseFixture:IDisposable
    {
        public readonly MovieExplorerDbContext dbContext;

        public DatabaseFixture()
        {
            var _dbName = Guid.NewGuid().ToString(); // Unique DB name per test class
            var options = new DbContextOptionsBuilder<MovieExplorerDbContext>()
                .UseInMemoryDatabase(_dbName)
                .Options;
            dbContext = new MovieExplorerDbContext(options);
        }

        public void Dispose()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }
    }
}

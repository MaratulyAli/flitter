using Flitter.Api.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flitter.Api.Testing.Helpers
{
    internal class MockDb : IDbContextFactory<FlitterDbContext>
    {
        public FlitterDbContext CreateDbContext()
        {
            var opts = new DbContextOptionsBuilder<FlitterDbContext>()
                .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
                .Options;

            return new FlitterDbContext(opts);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Chronoria_ConsumerWorkers.Tests.Models.Mocks.DbContexts
{
    public class InmemMockContext<T> where T : DbContext
    {
        public T context { get; set; }
        public InmemMockContext(Func<DbContextOptions<T>, T> construct)
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
            connection.Open();
            var optionsBuilder = new DbContextOptionsBuilder<T>();
            optionsBuilder.UseSqlite(connection);

            context = construct(optionsBuilder.Options);
        }
    }
}

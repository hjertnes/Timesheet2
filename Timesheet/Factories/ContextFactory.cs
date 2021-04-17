using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Timesheet.DataAccess;
using Timesheet.Utils;

namespace Timesheet.Factories
{
    public class ContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            optionsBuilder.UseSqlite(ExpandTilde.Expand(DatabaseConnectionFactory.Create()));

            return new Context(optionsBuilder.Options);
        }
    }
}
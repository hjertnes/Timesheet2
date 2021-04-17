using Microsoft.EntityFrameworkCore;
using Timesheet.DataAccess.Models;

namespace Timesheet.DataAccess
{
#nullable disable

    public class Context : DbContext
    {
        public Context (DbContextOptions<Context> options)
            : base(options)
        {
        }
        
        public DbSet<Entry> Entries { get; set; }
    }
#nullable enable
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.CommandInterface;
using Timesheet.DataAccess;
using Timesheet.Handlers;

namespace Timesheet
{
    public class ServiceProviderBuilder
    {
        private string _connectionString = "Data Source=Application.db;Cache=Shared";

        public ServiceProviderBuilder WithConnectionString(string databaseName)
        {
            _connectionString = $"Data Source={databaseName};Cache=Shared";
            return this;
        }

        public ServiceProvider Build()
        {
            return new ServiceCollection()
                .AddDbContext<Context>(x => x.UseSqlite(TildeUtils.Replace(_connectionString)))
                .AddSingleton<CommandFactory>()
                .AddSingleton<SummaryCommandFactory>()
                .AddSingleton<IMiscHandlers, MiscHandlers>()
                .AddSingleton<IEntryRepository, EntryRepository>()
                .AddSingleton<ISummaryRepository, SummaryRepository>()
                .AddSingleton<IEntryHandlers, EntryHandlers>()
                .AddSingleton<ISummaryHandlers, SummaryHandlers>()
                .AddSingleton<IMiscHandlers, MiscHandlers>()
                .BuildServiceProvider();
        }
    }
}
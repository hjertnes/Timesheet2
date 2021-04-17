using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.CommandInterface;
using Timesheet.DataAccess;
using Timesheet.ErrorHandling;
using Timesheet.Handlers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Timesheet
{
    public class YamlEvent
    {
        public string Start { get; set; }
        public string End { get; set; }
    }
    public class YamlEntry
    {
        public bool Excluded { get; set; }
        public List<YamlEvent> Events { get; set; }
    }
    //
    class Program
    {
        public static class TildeUtils
        {
            public static string Replace(string input)
            {
                return input.Replace("~", Environment.GetEnvironmentVariable("HOME"));
            }
        }
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
                    .AddSingleton<EntryCommandFactory>()
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

        static async Task Main(string[] args)
        {
            var provider = new ServiceProviderBuilder()
                .WithConnectionString("~/txt/timesheet.sqlite")
                .Build();
            var database = provider.GetService<Context>();
            if (database == null)
                throw new NullReferenceException();
            await database.Database.MigrateAsync();

            if (!database.Entries.Any())
            {
                var repo1 = provider.GetService<IEntryHandlers>();

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                    .Build();
                var text1 = File.ReadAllText("./timesheet.yaml");
                
                var data1 = deserializer.Deserialize<Dictionary<string, Dictionary<string, YamlEntry>>>(text1);
                
                foreach (var year in data1)
                {
                    foreach (var day in year.Value)
                    {
                        if (!day.Value.Events.Any())
                        {
                            await repo1!.Off(day.Key);
                        }
                        foreach (var e in day.Value.Events)
                        {
                            try
                            {
                                await repo1!.Add(day.Key, e.Start.Split(":")[0] + ":" + e.Start.Split(":")[1],
                                    e.End.Split(":")[0] + ":" + e.End.Split(":")[1], day.Value.Excluded);
                            }
                            catch (Error error)
                            {
                                Console.WriteLine(day.Key);
                                error.Print();
                            }
                            
                        }
                    }
                }
            }
            //
            var commandFactory = provider.GetService<CommandFactory>()?.Create() ?? throw new NullReferenceException();
            await commandFactory.InvokeAsync(args);
        }
    }
}
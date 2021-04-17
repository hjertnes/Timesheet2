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
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Timesheet
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var provider = new ServiceProviderBuilder()
                .WithConnectionString("~/txt/timesheet.sqlite")
                .Build();
            var database = provider.GetService<Context>();
            if (database == null)
                throw new NullReferenceException();
            await database.Database.MigrateAsync();
            var commandFactory = provider.GetService<CommandFactory>()?.Create() ?? throw new NullReferenceException();
            await commandFactory.InvokeAsync(args);
        }
    }
}
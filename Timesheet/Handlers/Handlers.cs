using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.DataAccess;
using Timesheet.Models;
using Timesheet.TablePrinter;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.Schemas;

namespace Timesheet.Handlers
{
    public interface IMiscHandlers
    {
        Task Export(string filename);
        Task Import(string filename, bool write);
    }

    public class MiscHandlers : IMiscHandlers
    {
        private readonly IEntryRepository _entryRepository;
        private readonly Context _context;

        public MiscHandlers(IEntryRepository entryRepository, Context context)
        {
            _context = context;
            _entryRepository = entryRepository;
        }
        
        public async Task Export(string filename)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize((await _entryRepository.GetAll()).Select(ExportModelFactory.Create));
            await File.WriteAllTextAsync(filename, yaml);
        }

        public async Task Import(string filename, bool write)
        {
            var text = await File.ReadAllTextAsync(filename);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();
            var p = deserializer.Deserialize<ExportModel[]>(text);
            

            if (write)
            {
                foreach (var x in p)
                {
                    var fromDate = DateTime.Parse($"{x.Date}T{x.Start}");
                    var toDate = DateTime.Parse($"{x.Date}T{x.End}");
                    await _entryRepository.Add(fromDate, toDate, x.Excluded, x.Off);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                var table = new DataTable("Would import");
                table.Columns.Add("Date", typeof(string));
                table.Columns.Add("Start", typeof(string));
                table.Columns.Add("End", typeof(string));
                table.Columns.Add("Off", typeof(string));
                table.Columns.Add("Excluded", typeof(string));
                foreach (var x in p)
                {
                    table.Rows.Add(
                        x.Date,
                        x.Start,
                        x.End,
                        x.Off ? "Yes" : "No",
                        x.Excluded ? "Yes" : "No");
                }
             
                Console.WriteLine(table.ToPrettyPrintedString());
            }
        }
    }
}
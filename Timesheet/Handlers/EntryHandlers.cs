using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Timesheet.DataAccess;
using Timesheet.TablePrinter;

namespace Timesheet.Handlers
{
    public interface IEntryHandlers
    {
        Task List();
        Task Delete(int entryId);
        Task Off(string date);
        Task Add(string date, string from, string to, bool excluded);
    }
    
    public class EntryHandlers : IEntryHandlers
    {
        private readonly Context _context;
        private readonly IEntryRepository _entryRepository;

        public EntryHandlers(IEntryRepository entryRepository, Context context)
        {
            _context = context;
            _entryRepository = entryRepository;
        }

        public async Task List()
        {
            var items = await _entryRepository.GetAll();
            if (!items.Any())
            {
                Console.WriteLine("No entries, you should add one.");
                return;
            }
            
            var table = new DataTable("Entries");
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("Date", typeof(string));
            table.Columns.Add("Start", typeof(string));
            table.Columns.Add("End", typeof(string));
            table.Columns.Add("Off", typeof(string));
            table.Columns.Add("Excluded", typeof(string));
            items.ForEach(x => table.Rows.Add(
                x.EntryId.ToString(),
                x.Start.ToString("yyyy-MM-dd"),
                x.Start.ToString("HH:mm"),
                x.End.ToString("HH:mm"),
                x.Off ? "Yes" : "No",
                x.Excluded ? "Yes" : "No")
            );
            Console.WriteLine(table.ToPrettyPrintedString());
        }

        public async Task Delete(int entryId)
        {
            _entryRepository.Delete(await _entryRepository.Get(entryId));
            await _context.SaveChangesAsync();
        }

        public async Task Off(string date)
        {
            if (!Regex.IsMatch(date, @"^\d{4}-\d{2}-\d{2}$"))
            {
                Console.WriteLine("Invalid formatted date");
                return;
            }

            var theDate = DateTime.Parse($"{date}T00:00:00");

            await _entryRepository.Add(theDate, theDate, false, true);
            await _context.SaveChangesAsync();
        }

        public async Task Add(string date, string @from, string to, bool excluded)
        {
            if (!Regex.IsMatch(date, @"^\d{4}-\d{2}-\d{2}$"))
            {
                Console.WriteLine("Invalid formatted date");
                return;
            }
            
            if (!Regex.IsMatch(from, @"^\d{2}:\d{2}$"))
            {
                Console.WriteLine("Invalid formatted from time");
                return;
            }
            
            if (!Regex.IsMatch(to, @"^\d{2}:\d{2}$"))
            {
                Console.WriteLine("Invalid formatted to time");
                return;
            }
            
            var fromDate = DateTime.Parse($"{date}T{from}:00");
            var toDate = DateTime.Parse($"{date}T{to}:00");

            await _entryRepository.Add(fromDate, toDate, excluded, false);
            await _context.SaveChangesAsync();
        }
    }
}
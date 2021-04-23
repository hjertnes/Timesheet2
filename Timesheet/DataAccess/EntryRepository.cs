using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Timesheet.DataAccess.Models;
using Timesheet.ErrorHandling;

namespace Timesheet.DataAccess
{
    public interface IEntryRepository
    {
        Task<Entry> Get(int id);
        Task<List<Entry>> GetAll();
        Task Add(DateTime start, DateTime end, bool excluded, bool off);
        void Delete(Entry entry);
    }
    
    public class EntryRepository : IEntryRepository
    {
        private readonly Context _context;

        public EntryRepository(Context context)
        {
            _context = context;
        }

        public async Task<Entry> Get(int id)
        {
            var entry = await _context.Entries.FirstOrDefaultAsync(x => x.EntryId == id);
            if (entry == null)
            {
                throw ErrorFactory.Create(ErrorType.NotFound, "Entry not found", $"Could not find an entry with ID='{id}'");
            }

            return entry;
        }

        public Task<List<Entry>> GetAll()
        {
            return _context.Entries.OrderBy(x => x.Start).ToListAsync();
        }

        public async Task Add(DateTime start, DateTime end, bool excluded, bool off)
        {
            if (start > end)
            {
                throw ErrorFactory.Create(ErrorType.InvalidInput, "Entry end is before start",
                    "You have probably mixed up start and end");
            }
            if (await _context.Entries.AnyAsync(x => x.Start >= start && x.End <= end))
            {
                throw ErrorFactory.Create(ErrorType.Overlap, "Entry overlaps", "The start and end overlaps with existing entry or entries");
            }

            var e = new Entry
            {
                Start = start,
                End = end,
                Off = off,
                Excluded = excluded,
            };

            if (_context.Entries.Any(x => x.Start.Date == start.Date && x.Excluded == true))
                e.Excluded = true;

            await _context.Entries.AddAsync(e);
        }

        public void Delete(Entry entry)
        {
            _context.Remove(entry);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Timesheet.Settings;

namespace Timesheet.DataAccess
{
    public interface ISummaryRepository
    {
        Task<TimeSpan> GetExpectedHoursAll();
        Task<TimeSpan> GetActualHoursAll();
        Task<Dictionary<int, TimeSpan>> GetExpectedHoursYear();
        Task<Dictionary<int, TimeSpan>> GetActualHoursYear();
        Task<Dictionary<string, TimeSpan>> GetActualHoursDay();
    }
    
    public class SummaryRepository : ISummaryRepository
    {
        private readonly Context _context;

        public SummaryRepository(Context context)
        {
            _context = context;
        }

        public async Task<TimeSpan> GetExpectedHoursAll()
        {
            return (await _context.Entries.Where(x => !x.Excluded).Select(x => x.Start.Date).Distinct().CountAsync()) *
                   SettingsProvider.GetWorkDay();
        }

        public async Task<TimeSpan> GetActualHoursAll()
        {
            return (await _context.Entries.ToListAsync())
                .Select(x => (x.Excluded || x.Off) ? x.End - x.Start : (x.End - x.Start) - SettingsProvider.GetBreakTime())
                .Aggregate((x, y) => x + y);
        }

        public async Task<Dictionary<int, TimeSpan>> GetExpectedHoursYear()
        {
            var result = new Dictionary<int, TimeSpan>();
            var years = _context.Entries.Where(x => !x.Excluded).Select(x => x.Start.Year).Distinct();
            foreach (var year in years)
            {
                result[year] = (await _context.Entries.Where(x => !x.Excluded).Where(x => x.Start.Year == year).Select(x => x.Start.Date).Distinct().CountAsync()) * SettingsProvider.GetWorkDay();
            }

            return result;
        }

        public async Task<Dictionary<int, TimeSpan>> GetActualHoursYear()
        {
            var result = new Dictionary<int, TimeSpan>();
            var years = await _context.Entries.Select(x => x.Start.Year).Distinct().ToListAsync();
            foreach (var year in years)
            {
                result[year] = _context.Entries
                    .Where(x => x.Start.Year == year)
                    .ToList()
                    .Select(x =>
                        {
                            if (x.Excluded)
                                return x.End - x.Start;
                            if (x.Off)
                                return TimeSpan.FromHours(0);
                            return (x.End - x.Start) - SettingsProvider.GetBreakTime();
                        })
                        .Aggregate((x, y) => x + y);
                Console.WriteLine(result[year].TotalHours);
            }

            return result;
        }

        public async Task<Dictionary<string, TimeSpan>> GetActualHoursDay()
        {
            var result = new Dictionary<string, TimeSpan>();
            var days = await _context.Entries.Select(x => x.Start.ToString("yyyy-MM-dd")).Distinct().ToListAsync();
            foreach (var day in days)
            {
                result[day] = _context.Entries.ToList()
                    .Where(x => x.Start.ToString("yyyy-MM-dd") == day)
                    
                    .Select(x =>
                        (x.Excluded || x.Off) ? x.End - x.Start : (x.End - x.Start) - SettingsProvider.GetBreakTime())
                    .Aggregate((x, y) => x + y);
            }

            return result;
        }
    }
}
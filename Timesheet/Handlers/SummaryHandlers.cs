using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.DataAccess;
using Timesheet.TablePrinter;

namespace Timesheet.Handlers
{
    public static class TimeSpanExt
    {
        private static string Positive(TimeSpan t)
        {
            var hours = 0;
            var minutes = 0;
            var c = t.TotalMinutes;
            while (true)
            {
                if (c < 60)
                {
                    minutes = (int)c;
                    break;
                }

                hours++;
                c -= 60;
            }

            return $"{hours}h {minutes}m";
        }
        
        private static string Negative(TimeSpan t)
        {
            var hours = 0;
            var minutes = 0;
            var c = (decimal)t.TotalMinutes;
            c = decimal.Negate(c);
            
            while (true)
            {
                if (c < 60)
                {
                    minutes = (int)c;
                    break;
                }

                hours++;
                c -= 60;
            }

            if (hours > 0)
                hours = -hours;
            if (minutes > 0)
                minutes = -minutes;

            return $"{hours}h {minutes}m";
        }
        
        public static string PrettyTimespan(this TimeSpan t)
        {
            if (t.TotalMinutes < 0)
                return Negative(t);
            return Positive(t);
        }
    }
    public interface ISummaryHandlers
    {
        Task All();
        Task Year();
        Task Day();
    }
    
    public class SummaryHandlers : ISummaryHandlers
    {
        private readonly ISummaryRepository _summaryRepository;

        public SummaryHandlers(ISummaryRepository summaryRepository)
        {
            _summaryRepository = summaryRepository;
        }

        public async Task All()
        {
            var expected = await _summaryRepository.GetExpectedHoursAll();
            var actual = await _summaryRepository.GetActualHoursAll();
            
            var table = new DataTable("Summary All");
            table.Columns.Add("Expected", typeof(string));
            table.Columns.Add("Actual", typeof(string));
            table.Columns.Add("Balance", typeof(string));
            table.Rows.Add(expected.PrettyTimespan(), actual.PrettyTimespan(), (actual - expected).PrettyTimespan());
            Console.WriteLine(table.ToPrettyPrintedString());
        }
        
        public async Task Year()
        {
            var expected = await _summaryRepository.GetExpectedHoursYear();
            var actual = await _summaryRepository.GetActualHoursYear();
            
            var table = new DataTable("Summary Year");
            table.Columns.Add("Year", typeof(string));
            table.Columns.Add("Expected", typeof(string));
            table.Columns.Add("Actual", typeof(string));
            table.Columns.Add("Balance", typeof(string));
            foreach (var year in expected.Keys.OrderBy(x => x))
            {
                var e = expected[year];
                var a = actual[year];
                table.Rows.Add(year.ToString(), e.PrettyTimespan(), a.PrettyTimespan(), (a - e).PrettyTimespan());
            }
            Console.WriteLine(table.ToPrettyPrintedString());
        }

        public async Task Day()
        {
            var days = await _summaryRepository.GetActualHoursDay();
            

            var table = new DataTable("Summary Day");
            table.Columns.Add("Day", typeof(string));
            table.Columns.Add("Hours", typeof(string));
            foreach (var day in days.OrderBy(x => x.Key))
            {
                table.Rows.Add(day.Key, day.Value.PrettyTimespan());
            }
            Console.WriteLine(table.ToPrettyPrintedString());
        }
    }
}
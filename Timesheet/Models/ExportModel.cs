using Timesheet.DataAccess.Models;

namespace Timesheet.Models
{
    public static class ExportModelFactory
    {
        public static ExportModel Create(Entry entry) => 
            new ExportModel
            {
                Date = entry.Start.ToString("yyyy-MM-dd"),
                Start = entry.Start.ToString("HH:mm:ss"),
                End = entry.End.ToString("HH:mm:ss"),
                Excluded = entry.Excluded,
                Off = entry.Off
            };
    }
    public class ExportModel
    {
        public string Date { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public bool Off { get; set; } 
        public bool Excluded { get; set; }
    }
}
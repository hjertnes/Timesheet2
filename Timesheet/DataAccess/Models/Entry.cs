using System;

namespace Timesheet.DataAccess.Models
{
    public class Entry
    {
        public int EntryId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        
        public bool Excluded { get; set; }
        public bool Off { get; set; }
    }
}
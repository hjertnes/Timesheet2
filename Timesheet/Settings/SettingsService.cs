using System;

namespace Timesheet.Settings
{
    public static class SettingsProvider
    {
        public static TimeSpan GetBreakTime()
        {
            return TimeSpan.FromMinutes(int.Parse(Environment.GetEnvironmentVariable("TIMESHEET_BREAK") ?? "30"));
        }

        public static TimeSpan GetWorkDay()
        {
            return TimeSpan.FromMinutes(int.Parse(Environment.GetEnvironmentVariable("TIMESHEET_DAY") ?? "450"));
        }
    }
}
using System;

namespace Timesheet.Utils
{
    public static class ExpandTilde
    {
        public static string Expand(string input)
        {
            return input.Replace("~", Environment.GetEnvironmentVariable("HOME"));
        }
    }
}
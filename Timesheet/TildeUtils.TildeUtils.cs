using System;

namespace Timesheet
{
    public static class TildeUtils
    {
        public static string Replace(string input)
        {
            return input.Replace("~", Environment.GetEnvironmentVariable("HOME"));
        }
    }
}
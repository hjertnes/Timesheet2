using System;

namespace Timesheet.Factories
{
    public static class DatabaseConnectionFactory
    {
        public static string Create()
        {
            if (Environment.GetEnvironmentVariable("TEST") != "")
            {
                return "Data Source=~/.timesheet-test.db;";
            }
            
            return "Data Source=~/.timesheet.db;";
        }
    }
}
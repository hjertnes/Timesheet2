using System;
using System.Data;
using Timesheet.TablePrinter;

namespace Timesheet.ErrorHandling
{
    public class Error : Exception
    {
        public ErrorType ErrorType { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }

        public void Print()
        {
            Console.WriteLine("Shit hit the fan");
            DataTable table = new DataTable("Error Details");
            table.Rows.Add("Title", Title);
            table.Rows.Add("Detail", Detail);
            table.Rows.Add("Type", ErrorType.ToString());
            Console.WriteLine(table.ToPrettyPrintedString());
        }
    }
}
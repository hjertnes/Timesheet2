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
            Console.WriteLine("Error Details");
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Detail: {Detail}");
        }
    }
}
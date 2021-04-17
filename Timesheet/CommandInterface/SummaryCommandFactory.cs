using System.CommandLine;
using System.CommandLine.Invocation;
using Timesheet.Handlers;

namespace Timesheet.CommandInterface
{
    public class SummaryCommandFactory
    {
        private readonly ISummaryHandlers _summaryHandlers;

        public SummaryCommandFactory(ISummaryHandlers summaryHandlers)
        {
            _summaryHandlers = summaryHandlers;
        }

        public Command Create()
        {
            return new Command("summary", "shows sums of hours per employer per time unit")
            {
                new Command("all", "time unit is in total")
                {
                    Handler = CommandHandler.Create(async () => await _summaryHandlers.All())
                },
                new Command("day", "time unit is per day")
                {
                    Handler = CommandHandler.Create(async () => await _summaryHandlers.Day())
                },
                new Command("year", "time unit is per year")
                {
                    Handler = CommandHandler.Create(async () => await _summaryHandlers.Year())
                },
            };
        }
    }
}
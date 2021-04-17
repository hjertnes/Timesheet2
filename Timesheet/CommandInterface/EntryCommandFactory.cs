using System.CommandLine;
using System.CommandLine.Invocation;
using Timesheet.Handlers;

namespace Timesheet.CommandInterface
{
    public class EntryCommandFactory
    {
        private readonly IEntryHandlers _entryHandlers;

        public EntryCommandFactory(IEntryHandlers entryHandlers)
        {
            _entryHandlers = entryHandlers;
        }
        public Command Create()
        {
            return new Command("entries", "entries are the hourly logs")
            {
                Add(),
                Off(),
                Delete(),
                List(),
            };
        }

        Command Add()
        {
            var c = new Command("add", "adds a entry")
            {
                new Argument<string>("date")
                {
                    Description = "the date",
                },
                new Argument<string>("start")
                {
                    Description = "start time",
                },
                new Argument<string>("end")
                {
                    Description = "end time",
                },
                new Option<bool>("--excluded", "if this date should not be treated as a regular work day"),
            };
		    
            c.Handler = CommandHandler.Create(async (string date, string start, string end, bool excluded) => await _entryHandlers.Add(date, start, end, excluded));

            return c;
        }

        Command Off()
        {
            var c = new Command("off", "adds a day off")
            {
                new Argument<string>("date")
                {
                    Description = "adds a day off"
                }
            };
		    
            c.Handler = CommandHandler.Create(async (string date) =>
            {
                await _entryHandlers.Off(date);
            });

            return c;
        }

        Command Delete()
        {
            var c = new Command("delete", "deleted a entry")
            {
                new Argument<int>("entryId")
                {
                    Description = "the id of the entry",
                },
            };
		    
            c.Handler = CommandHandler.Create(async (int entryId) => await _entryHandlers.Delete(entryId));

            return c;
        }

        Command List()
        {
            var c = new Command("list", "lists out all entries");
            c.Handler = CommandHandler.Create(async () => await _entryHandlers.List());
            return c;
        }
    }
}
using System.CommandLine;
using System.CommandLine.Invocation;
using Timesheet.Handlers;

namespace Timesheet.CommandInterface
{
    public class CommandFactory
    {
        private readonly IEntryHandlers _entryHandlers;
        private readonly SummaryCommandFactory _summaryCommandFactory;
        private readonly IMiscHandlers _miscHandlers;

        public CommandFactory(
            IEntryHandlers entryHandlers,
            SummaryCommandFactory summaryCommandFactory, 
            IMiscHandlers miscHandlers)
        {
            _entryHandlers = entryHandlers;
            _summaryCommandFactory = summaryCommandFactory;
            _miscHandlers = miscHandlers;
        }

        private Command Import()
        {
            var c = new Command("import", "imports a file")
            {
                new Argument<string>("filename")
                {
                    Description = "the filename of the file you want to import"
                },
                new Option<bool>("--dry", "parses and does everything except saving it to the database")
            };
		    
            c.Handler = CommandHandler.Create(async (string filename, bool dry) => await _miscHandlers.Import(filename, dry));

            return c;
        }

        private Command Export()
        {
            var c = new Command("export", "exports data to yaml")
            {
                new Argument<string>("filename")
                {
                    Description = "the filename of the file you want to import"
                },
            };
            c.Handler = CommandHandler.Create(async (string filename) => await _miscHandlers.Export(filename));
            return c;
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
	    
        public RootCommand Create()
        {
            var root = new RootCommand("a simple utility to keep a timesheet from the cli")
            {
                _summaryCommandFactory.Create(),
                Import(),
                Export(),
                Add(),
                Off(),
                Delete(),
                List(),
            };

            root.Name = "timesheet";

            return root;
        }
    }
}
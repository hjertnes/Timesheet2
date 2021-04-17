using System.CommandLine;
using System.CommandLine.Invocation;
using Timesheet.Handlers;

namespace Timesheet.CommandInterface
{
    public class CommandFactory
    {
        private readonly EntryCommandFactory _entryCommandFactory;
        private readonly SummaryCommandFactory _summaryCommandFactory;
        private readonly IMiscHandlers _miscHandlers;

        public CommandFactory(
            EntryCommandFactory entryCommandFactory, 
            SummaryCommandFactory summaryCommandFactory, 
            IMiscHandlers miscHandlers)
        {
            _entryCommandFactory = entryCommandFactory;
            _summaryCommandFactory = summaryCommandFactory;
            _miscHandlers = miscHandlers;
        }

        private Command Rematch()
        {
            var c = new Command("rematch", "will re-match entries to timespans");
            c.Handler = CommandHandler.Create(async () => await _miscHandlers.Rematch());
            return c;
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
	    
        public RootCommand Create()
        {
            var root = new RootCommand("a simple utility to keep a timesheet from the cli")
            {
                _entryCommandFactory.Create(),
                _summaryCommandFactory.Create(),
                Rematch(),
                Import(),
                Export(),
            };

            root.Name = "timesheet";

            return root;
        }
    }
}
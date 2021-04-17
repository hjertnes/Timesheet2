using System.Threading.Tasks;

namespace Timesheet.Handlers
{
    public interface IMiscHandlers
    {
        Task Export(string filename);
        Task Import(string filename, bool dry);
        Task Rematch();
    }

    public class MiscHandlers : IMiscHandlers
    {
        public Task Export(string filename)
        {
            throw new System.NotImplementedException();
        }

        public Task Import(string filename, bool dry)
        {
            throw new System.NotImplementedException();
        }

        public Task Rematch()
        {
            throw new System.NotImplementedException();
        }
    }
}
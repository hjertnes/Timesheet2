namespace Timesheet.ErrorHandling
{
    public static class ErrorFactory
    {
        public static Error Create(ErrorType type, string title, string detail)
        {
            return new Error
            {
                ErrorType = type,
                Title = title,
                Detail = detail,
            };
        }
    }
}
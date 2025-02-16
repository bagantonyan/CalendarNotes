namespace CalendarNotes.API.Extensions
{
    public static class StatusCodeExtensions
    {
        public static bool IsSuccess(this int statusCode) => statusCode >= 200 && statusCode <= 299;
    }
}
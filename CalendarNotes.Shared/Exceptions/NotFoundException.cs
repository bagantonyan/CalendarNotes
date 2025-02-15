namespace CalendarNotes.Shared.Exceptions
{
    public abstract class NotFoundException(string message) : Exception(message)
    {
    }
}

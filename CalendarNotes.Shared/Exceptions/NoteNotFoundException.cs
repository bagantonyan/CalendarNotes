namespace CalendarNotes.Shared.Exceptions
{
    public class NoteNotFoundException(int id) : NotFoundException($"Note with id {id} not found")
    {
    }
}
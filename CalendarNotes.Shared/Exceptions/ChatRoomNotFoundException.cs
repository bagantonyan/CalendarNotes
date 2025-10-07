namespace CalendarNotes.Shared.Exceptions
{
    public class ChatRoomNotFoundException(int id) : NotFoundException($"Комната чата с ID {id} не найдена")
    {
    }
}


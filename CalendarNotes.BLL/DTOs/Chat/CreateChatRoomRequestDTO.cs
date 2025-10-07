namespace CalendarNotes.BLL.DTOs.Chat
{
    /// <summary>
    /// DTO для создания комнаты чата
    /// </summary>
    public class CreateChatRoomRequestDTO
    {
        public string? Name { get; set; }
        public bool IsGroupChat { get; set; }
        public List<string> ParticipantIds { get; set; } = new();
    }
}


namespace CalendarNotes.BLL.DTOs.Chat
{
    /// <summary>
    /// DTO для отображения комнаты чата
    /// </summary>
    public class ChatRoomResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsGroupChat { get; set; }
        public string CreatorUserId { get; set; } = string.Empty;
        public List<string> ParticipantIds { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ChatMessageResponseDTO? LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }
}


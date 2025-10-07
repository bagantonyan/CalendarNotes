namespace CalendarNotes.API.Models.Chat
{
    /// <summary>
    /// Модель ответа для сообщения чата
    /// </summary>
    public class ChatMessageResponseModel
    {
        public int Id { get; set; }
        public int ChatRoomId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}


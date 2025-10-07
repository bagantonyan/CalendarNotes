namespace CalendarNotes.BLL.DTOs.Chat
{
    /// <summary>
    /// DTO для отображения сообщения чата
    /// </summary>
    public class ChatMessageResponseDTO
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


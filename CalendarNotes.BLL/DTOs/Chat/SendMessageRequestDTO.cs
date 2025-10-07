namespace CalendarNotes.BLL.DTOs.Chat
{
    /// <summary>
    /// DTO для отправки сообщения
    /// </summary>
    public class SendMessageRequestDTO
    {
        public int ChatRoomId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}


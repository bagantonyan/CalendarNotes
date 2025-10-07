namespace CalendarNotes.DAL.Entities
{
    /// <summary>
    /// Сообщение в чате
    /// </summary>
    public class ChatMessage : BaseEntity
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ID комнаты чата
        /// </summary>
        public int ChatRoomId { get; set; }
        
        /// <summary>
        /// ID отправителя
        /// </summary>
        public string SenderId { get; set; } = string.Empty;
        
        /// <summary>
        /// Имя отправителя (для отображения)
        /// </summary>
        public string SenderName { get; set; } = string.Empty;
        
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Content { get; set; } = string.Empty;
        
        /// <summary>
        /// Признак прочитанного сообщения
        /// </summary>
        public bool IsRead { get; set; }
        
        /// <summary>
        /// Комната чата
        /// </summary>
        public virtual ChatRoom ChatRoom { get; set; } = null!;
    }
}


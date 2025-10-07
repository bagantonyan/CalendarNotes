namespace CalendarNotes.DAL.Entities
{
    /// <summary>
    /// Комната чата для общения между пользователями
    /// </summary>
    public class ChatRoom : BaseEntity
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Название комнаты (опционально для групповых чатов)
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Признак группового чата
        /// </summary>
        public bool IsGroupChat { get; set; }
        
        /// <summary>
        /// ID создателя комнаты
        /// </summary>
        public string CreatorUserId { get; set; } = string.Empty;
        
        /// <summary>
        /// Участники чата (ID пользователей через запятую)
        /// </summary>
        public string ParticipantIds { get; set; } = string.Empty;
        
        /// <summary>
        /// Сообщения в этой комнате
        /// </summary>
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}


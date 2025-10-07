using CalendarNotes.DAL.Entities;

namespace CalendarNotes.DAL.Repositories.Interfaces
{
    public interface IChatMessageRepository : IBaseRepository<ChatMessage>
    {
        /// <summary>
        /// Получить сообщение по ID
        /// </summary>
        Task<ChatMessage?> GetByIdAsync(int messageId, bool trackChanges);
        
        /// <summary>
        /// Получить сообщения комнаты с пагинацией
        /// </summary>
        Task<List<ChatMessage>> GetRoomMessagesAsync(int roomId, int skip, int take, bool trackChanges);
        
        /// <summary>
        /// Получить количество непрочитанных сообщений в комнате
        /// </summary>
        Task<int> GetUnreadCountAsync(int roomId);
        
        /// <summary>
        /// Отметить все сообщения как прочитанные
        /// </summary>
        Task MarkAllAsReadAsync(int roomId);
    }
}


using CalendarNotes.BLL.DTOs.Chat;

namespace CalendarNotes.BLL.Services.Interfaces
{
    public interface IChatService
    {
        /// <summary>
        /// Создать новую комнату чата
        /// </summary>
        Task<ChatRoomResponseDTO> CreateRoomAsync(string creatorUserId, CreateChatRoomRequestDTO requestDTO);
        
        /// <summary>
        /// Получить все комнаты пользователя
        /// </summary>
        Task<List<ChatRoomResponseDTO>> GetUserRoomsAsync(string userId);
        
        /// <summary>
        /// Получить комнату по ID
        /// </summary>
        Task<ChatRoomResponseDTO> GetRoomByIdAsync(int roomId);
        
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        Task<ChatMessageResponseDTO> SendMessageAsync(string senderId, string senderName, SendMessageRequestDTO requestDTO);
        
        /// <summary>
        /// Получить сообщения комнаты
        /// </summary>
        Task<List<ChatMessageResponseDTO>> GetRoomMessagesAsync(int roomId, int skip = 0, int take = 50);
        
        /// <summary>
        /// Отметить сообщения как прочитанные
        /// </summary>
        Task MarkMessagesAsReadAsync(int roomId);
        
        /// <summary>
        /// Найти или создать приватную комнату между двумя пользователями
        /// </summary>
        Task<ChatRoomResponseDTO> GetOrCreatePrivateRoomAsync(string userId1, string userId2);
    }
}


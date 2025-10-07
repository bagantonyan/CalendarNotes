using CalendarNotes.DAL.Entities;

namespace CalendarNotes.DAL.Repositories.Interfaces
{
    public interface IChatRoomRepository : IBaseRepository<ChatRoom>
    {
        /// <summary>
        /// Получить комнату чата по ID
        /// </summary>
        Task<ChatRoom?> GetByIdAsync(int roomId, bool trackChanges);
        
        /// <summary>
        /// Получить все комнаты пользователя
        /// </summary>
        Task<List<ChatRoom>> GetUserRoomsAsync(string userId, bool trackChanges);
        
        /// <summary>
        /// Найти или создать приватную комнату между двумя пользователями
        /// </summary>
        Task<ChatRoom?> FindPrivateRoomAsync(string userId1, string userId2, bool trackChanges);
    }
}


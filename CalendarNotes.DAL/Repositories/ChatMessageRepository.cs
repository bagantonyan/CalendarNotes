using CalendarNotes.DAL.Contexts;
using CalendarNotes.DAL.Entities;
using CalendarNotes.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CalendarNotes.DAL.Repositories
{
    public class ChatMessageRepository(CalendarNotesDbContext dbContext) 
        : BaseRepository<ChatMessage>(dbContext), IChatMessageRepository
    {
        private readonly CalendarNotesDbContext _dbContext = dbContext;

        public async Task<ChatMessage?> GetByIdAsync(int messageId, bool trackChanges)
            => await GetByCondition(m => m.Id == messageId, trackChanges)
                .SingleOrDefaultAsync();

        public async Task<List<ChatMessage>> GetRoomMessagesAsync(int roomId, int skip, int take, bool trackChanges)
            => await GetByCondition(m => m.ChatRoomId == roomId, trackChanges)
                .OrderByDescending(m => m.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

        public async Task<int> GetUnreadCountAsync(int roomId)
            => await GetByCondition(m => m.ChatRoomId == roomId && !m.IsRead, false)
                .CountAsync();

        public async Task MarkAllAsReadAsync(int roomId)
        {
            await _dbContext.ChatMessages
                .Where(m => m.ChatRoomId == roomId && !m.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true));
        }
    }
}

